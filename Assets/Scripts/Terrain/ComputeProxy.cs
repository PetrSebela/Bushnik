using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Terrain.Data;
using Unity.Mathematics;
using UnityEngine.Rendering;
using Utility;

namespace Terrain
{
    /// <summary>
    /// Class responsible for interfacing with terrain compute shader
    /// </summary>
    public class ComputeProxy : MonoBehaviour
    {
        /// <summary>
        /// Private singleton instance
        /// </summary>
        private static ComputeProxy _instance;
        
        /// <summary>
        /// Singleton getter 
        /// </summary>
        public static ComputeProxy Instance => _instance;

        /// <summary>
        /// Compute shader used for terrain generation
        /// </summary>
        public ComputeShader TerrainComputeShader;

        public bool AllWorkersFree => _freeWorkers.Count == _workers.Count;
        
        /// <summary>
        /// ID of mesh kernel
        /// </summary>
        private int _terrainMeshKernel;

        /// <summary>
        /// ID of heightmap sample kernel
        /// </summary>
        private int _sampleKernel;
        
        /// <summary>
        /// ID of preview kernel
        /// </summary>
        private int _previewKernel;
        
        /// <summary>
        /// Compute buffer storing biome data
        /// </summary>
        private ComputeBuffer _airstripBuffer;
        
        /// <summary>
        /// TODO: move dependency management to Terrain and Pipeline management object
        /// </summary>
        public MeshSettings meshSettings;
        public TerrainSettings terrainSettings;
        public TerrainFeatureManager terrainFeatureManager;

        /// <summary>
        /// List of all used terrain workers
        /// </summary>
        private readonly List<TerrainWorker> _workers = new();
        
        /// <summary>
        /// List of available terrain workers
        /// </summary>
        private readonly List<TerrainWorker> _freeWorkers = new();

        /// <summary>
        /// Singleton initialization
        /// </summary>
        /// <exception cref="Exception">Exception when there would be two singletons</exception>
        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                throw new Exception("Duplicate instance of TerrainGenerator");
        }

        /// <summary>
        /// Sets up terrain generator
        /// </summary>
        public void Init()
        {
            GetKernelIDs();
            
            for (int i = 0; i < 5; i++)
            {
                var worker = new TerrainWorker(meshSettings, TerrainComputeShader);
                worker.OnTaskFinished += OnWorkerCompleted;
                _freeWorkers.Add(worker);
                _workers.Add(worker);
            }
            
            UpdateTerrainSettings();
        }

        private void OnWorkerCompleted(TerrainWorker worker)
        {
            _freeWorkers.Add(worker);
        }
        
        /// <summary>
        /// Creates and sets terrain affector
        /// This method requires
        /// </summary>
        public void UpdateTerrainAffectors(RunwayData[] affectors)
        {
            if(affectors.Length == 0)
                return;
            
            UpdateTerrainSettings();
            
            // Create compute buffer
            int runwayDataSize = (sizeof(float) * 3) * 2 + sizeof(float);
            _airstripBuffer?.Dispose();
            _airstripBuffer = new ComputeBuffer(affectors.Length, runwayDataSize);
            _airstripBuffer.SetData(affectors);
            
            // Set data to kernels
            TerrainComputeShader.SetInt("AirstripBufferSize", affectors.Length);
            SetBufferToAllKernels(_airstripBuffer, "AirstripBuffer");
        }

        /// <summary>
        /// Updates kernel ids
        /// </summary>
        private void GetKernelIDs()
        {
            _terrainMeshKernel = TerrainComputeShader.FindKernel("TerrainMesh");
            _sampleKernel = TerrainComputeShader.FindKernel("SamplePoints");
            _previewKernel = TerrainComputeShader.FindKernel("PreviewHeightmap");
        }

        /// <summary>
        /// Releases used compute buffers 
        /// </summary>
        private void OnDestroy()
        {
            foreach (var worker in _workers)
                worker.Dispose();
            
            _airstripBuffer?.Dispose();
        }

        /// <summary>
        /// Sends common terrain settings to the GPU
        /// </summary>
        private void UpdateTerrainSettings()
        {
            GetKernelIDs();
            TerrainComputeShader.SetFloat("TerrainSize", terrainSettings.size);
            TerrainComputeShader.SetInt("MeshResolution", meshSettings.resolution);
            TerrainComputeShader.SetFloat("TerrainHeight", terrainSettings.height);
            
            // Noise parameters
            TerrainComputeShader.SetInt("NoiseLayers", terrainSettings.noiseLayers);
            TerrainComputeShader.SetFloat("BaseNoiseFrequency", terrainSettings.baseNoiseFrequency);
            TerrainComputeShader.SetFloat("FrequencyDecay", terrainSettings.frequencyDecay);
            TerrainComputeShader.SetFloat("AmplitudeDecay", terrainSettings.amplitudeDecay);
            
            // Default terrain does not account for runways and other affectors
            TerrainComputeShader.SetInt("AirstripBufferSize", 0);
            _airstripBuffer?.Dispose();
            _airstripBuffer = new ComputeBuffer(1, 1);
            SetBufferToAllKernels(_airstripBuffer, "AirstripBuffer");
        }

        /// <summary>
        /// Sets passed buffer to be used across all kernels
        /// </summary>
        /// <param name="buffer">Buffer to be set</param>
        /// <param name="bname">Name of said buffer</param>
        private void SetBufferToAllKernels(ComputeBuffer buffer, string bname)
        {
            TerrainComputeShader.SetBuffer(_terrainMeshKernel, bname, buffer);
            TerrainComputeShader.SetBuffer(_previewKernel, bname, buffer);
            TerrainComputeShader.SetBuffer(_sampleKernel, bname, buffer);
        }

        /// <summary>
        /// Creates request for compute buffer readback
        /// </summary>
        /// <param name="buffer">Compute buffer to be read</param>
        /// <typeparam name="T">Datatype stored in compute buffer</typeparam>
        /// <returns>Task corresponding to said request</returns>

        
        public bool HasFreeWorker => _freeWorkers.Count > 0;

        /// <summary>
        /// Seizes single worker and tasks it with computing terrain mesh for said chunk
        /// </summary>
        /// <param name="chunk">Chunk for which the mesh will be computed</param>
        public void GetTerrainMesh(Chunk chunk)
        {
            var worker = _freeWorkers[0];
            _freeWorkers.RemoveAt(0);
            _ = worker.ComputeMesh(chunk);
        }

        /// <summary>
        /// Creates terrain heightmap texture (for visualization only)
        /// </summary>
        /// <param name="previewSize">Size of preview texture </param>
        /// <returns>Terrain heightmap</returns>
        public RenderTexture PreviewHeightmap(int previewSize)
        {
            UpdateTerrainSettings();
            int previewKernelIndex = TerrainComputeShader.FindKernel("PreviewHeightmap");

            var preview = new RenderTexture(previewSize, previewSize, 0, RenderTextureFormat.ARGB32);
            preview.enableRandomWrite = true;
            preview.Create();

            TerrainComputeShader.SetTexture(previewKernelIndex, "HeightmapPreview", preview);
            TerrainComputeShader.SetInt("PreviewSize", previewSize);

            int groups = previewSize / 32;
            TerrainComputeShader.Dispatch(previewKernelIndex, groups, 1, groups);
            return preview;
        }

        /// <summary>
        /// Blocking function for point sampling
        /// </summary>
        /// <param name="points"></param>
        public void SamplePoints(ref Vector3[] points)
        {
            int sampleKernel = TerrainComputeShader.FindKernel("SamplePoints");
            var buffer = new ComputeBuffer(points.Length, sizeof(float) * 3);
            buffer.SetData(points);
            
            TerrainComputeShader.SetBuffer(sampleKernel, "Points", buffer);
            TerrainComputeShader.SetInt("PointsSize", points.Length);
            
            TerrainComputeShader.SetFloat("AngleLimit", 90f);
            TerrainComputeShader.SetFloat("MaxHeight", terrainSettings.height);
            TerrainComputeShader.SetBool("IgnoreAirstrips", true);
            
            int groups = Mathf.CeilToInt(points.Length / 32f);
            TerrainComputeShader.Dispatch(sampleKernel, groups, 1, 1);
            
            buffer.GetData(points);
            buffer.Dispose();
        }
        
        /// <summary>
        /// Samples heightmap at specified positions
        /// </summary>
        /// <param name="points">Reference to array of points, their .y component will be modified</param>
        /// <returns>List of points that are valid</returns>
        public async Task SamplePoints(Vector3[] points, Action<Vector3[]> callback)
        {
            int sampleKernel = TerrainComputeShader.FindKernel("SamplePoints");
            var buffer = new ComputeBuffer(points.Length, sizeof(float) * 3);
            buffer.SetData(points);
            
            TerrainComputeShader.SetBuffer(sampleKernel, "Points", buffer);
            TerrainComputeShader.SetInt("PointsSize", points.Length);
            
            TerrainComputeShader.SetFloat("AngleLimit", 90f);
            TerrainComputeShader.SetFloat("MaxHeight", terrainSettings.height);
            TerrainComputeShader.SetBool("IgnoreAirstrips", true);
            
            int groups = Mathf.CeilToInt(points.Length / 32f);
            TerrainComputeShader.Dispatch(sampleKernel, groups, 1, 1);
            
            var pointsRequest = GPUUtils.ReadBufferAsync<Vector3>(buffer);
            await Task.WhenAll(pointsRequest);
            points = pointsRequest.Result;
            
            buffer.Dispose();
            
            callback.Invoke(points);
        }
        
        /// <summary>
        /// Samples points for specified foliage object
        /// </summary>
        /// <param name="points"></param>
        /// <param name="foliage"></param>
        /// <returns></returns>
        public async Task SamplePoints(Vector3[] points, Action<Vector3[]> callback, Foliage.Foliage foliage )
        {
            int sampleKernel = TerrainComputeShader.FindKernel("SamplePoints");
            var buffer = new ComputeBuffer(points.Length, sizeof(float) * 3);
            buffer.SetData(points);
            
            TerrainComputeShader.SetBuffer(sampleKernel, "Points", buffer);
            TerrainComputeShader.SetInt("PointsSize", points.Length);
            
            // Foliage parameters
            TerrainComputeShader.SetFloat("AngleLimit", foliage.maxAngle);
            TerrainComputeShader.SetFloat("MaxHeight", foliage.maxHeight);
            TerrainComputeShader.SetBool("IgnoreAirstrips", false);
            
            int groups = Mathf.CeilToInt(points.Length / 32f);
            TerrainComputeShader.Dispatch(sampleKernel, groups, 1, 1);
            
            var pointsRequest = GPUUtils.ReadBufferAsync<Vector3>(buffer);
            await Task.WhenAll(pointsRequest);
            points = pointsRequest.Result;
            
            buffer.Dispose();
            
            var filtered = points.Where(point => point.y >= 0).ToArray();
            callback.Invoke(filtered);
        }
    }
}