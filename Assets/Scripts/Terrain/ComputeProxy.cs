using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Terrain.Data;
using Unity.Mathematics;

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

        [Tooltip("Compute shader used for generation of terrain heightmap and meshes")]
        public ComputeShader TerrainComputeShader;

        /// <summary>
        /// Index of terrain mesh kernel (heightmap + meshes)
        /// </summary>
        private int _terrainMeshKernel;

        /// <summary>
        /// Index of sample kernel
        /// </summary>
        private int _sampleKernel;
        
        /// <summary>
        /// Index of preview kernel
        /// </summary>
        private int _previewKernel;
        
        /// <summary>
        /// Compute buffer containing mesh vertices
        /// </summary>
        private ComputeBuffer _terrainVertexBuffer;
        
        /// <summary>
        /// Compute buffer containing mesh indices
        /// </summary>
        private ComputeBuffer _terrainIndexBuffer;
        
        /// <summary>
        /// Compute buffer containing terrain normals
        /// </summary>
        private ComputeBuffer _terrainNormalBuffer;
        
        /// <summary>
        /// Compute buffer containing additional terrain data that are passed to the fragment and vertex shaders (use uv0)
        /// </summary>
        private ComputeBuffer _terrainDataBuffer;
        
        /// <summary>
        /// Compute buffer storing biome data
        /// </summary>
        private ComputeBuffer _airstripBuffer;

        [Tooltip("Mesh settings")]
        public MeshSettings meshSettings;
        
        [Tooltip("Terrain settings")]
        public TerrainSettings terrainSettings;

        [Tooltip("Terrain feature manager")]
        public TerrainFeatureManager terrainFeatureManager;

        /// <summary>
        /// Count thread groups on single axis for mesh generation
        /// </summary>
        private int ThreadGroups = 1;

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
        private void Start()
        {
            GetKernelIDs();
            int vertexCount = (int)Mathf.Pow(meshSettings.resolution, 2);
            int indicesCount = (int)Mathf.Pow(meshSettings.resolution - 1, 2) * 6;

            _terrainVertexBuffer = new ComputeBuffer(vertexCount, sizeof(float) * 3);
            _terrainIndexBuffer = new ComputeBuffer(indicesCount, sizeof(uint));
            _terrainNormalBuffer = new ComputeBuffer(vertexCount, sizeof(float) * 3);
            _terrainDataBuffer = new ComputeBuffer(vertexCount, sizeof(float) * 2);

            TerrainComputeShader.SetBuffer(_terrainMeshKernel, "VertexBuffer", _terrainVertexBuffer);
            TerrainComputeShader.SetBuffer(_terrainMeshKernel, "IndexBuffer", _terrainIndexBuffer);
            TerrainComputeShader.SetBuffer(_terrainMeshKernel, "NormalBuffer", _terrainNormalBuffer);
            TerrainComputeShader.SetBuffer(_terrainMeshKernel, "DataBuffer", _terrainDataBuffer);
            
            ThreadGroups = Mathf.CeilToInt(meshSettings.resolution / 32f);
            

            
            UpdateTerrainSettings();
            UpdateTerrainAffectors();
        }

        /// <summary>
        /// Creates and sets terrain affector
        /// This method requires
        /// </summary>
        private void UpdateTerrainAffectors()
        {
            UpdateTerrainSettings();

            var runways = terrainFeatureManager.GetRunways();

            // Create compute buffer
            int runwayDataSize = (sizeof(float) * 3) * 2 + sizeof(float);
            _airstripBuffer?.Dispose();
            _airstripBuffer = new ComputeBuffer(runways.Length, runwayDataSize);
            _airstripBuffer.SetData(runways);
            
            // Set data to kernels
            TerrainComputeShader.SetInt("AirstripBufferSize", runways.Length);
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
            _terrainVertexBuffer?.Dispose();
            _terrainIndexBuffer?.Dispose();
            _terrainNormalBuffer?.Dispose();
            _terrainDataBuffer?.Dispose();
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
        /// Constructs terrain mesh and heightmap using compute shaders for given chunk
        /// </summary>
        /// <param name="position">chunk position</param>
        /// <param name="size">chunk size</param>
        /// <param name="depth">chunk depth in LOD tree</param>
        /// <returns></returns>
        public Mesh GetTerrainMesh(Vector3 position, float size, int depth)
        {
            TerrainComputeShader.SetFloat("ChunkSize", size);
            TerrainComputeShader.SetFloats("ChunkPosition", position.x, position.y, position.z);
            TerrainComputeShader.SetInt("ChunkDepth", depth);

            TerrainComputeShader.Dispatch(_terrainMeshKernel, ThreadGroups, 1, ThreadGroups);

            Vector3[] vertices = new Vector3[_terrainVertexBuffer.count];
            _terrainVertexBuffer.GetData(vertices);

            int[] indices = new int[_terrainIndexBuffer.count];
            _terrainIndexBuffer.GetData(indices);

            Vector3[] normals = new Vector3[_terrainNormalBuffer.count];
            _terrainNormalBuffer.GetData(normals);

            Vector2[] data = new Vector2[_terrainDataBuffer.count];
            _terrainDataBuffer.GetData(data);

            Mesh mesh = new()
            {
                vertices = vertices,
                triangles = indices,
                normals = normals,
                uv = data
            };
            
            // TODO: do normal calculation on GPU to avoid mesh seams
            // mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            return mesh;
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
        /// Samples heightmap at specified positions
        /// </summary>
        /// <param name="points">Reference to array of points, their .y component will be modified</param>
        /// <returns>List of points that are valid</returns>
        public Vector3[] SamplePoints(ref Vector3[] points, float maxAngle = 90f)
        {
            int sampleKernel = TerrainComputeShader.FindKernel("SamplePoints");
            var buffer = new ComputeBuffer(points.Length, sizeof(float) * 3);
            buffer.SetData(points);
            
            TerrainComputeShader.SetBuffer(sampleKernel, "Points", buffer);
            TerrainComputeShader.SetInt("PointsSize", points.Length);
            TerrainComputeShader.SetFloat("AngleLimit", maxAngle);
            
            
            int groups = Mathf.CeilToInt(points.Length / 32f);
            TerrainComputeShader.Dispatch(sampleKernel, groups, 1, 1);
            
            buffer.GetData(points);
            buffer.Dispose();

            return points.Where(point => point.y >= 0).ToArray();
        }
    }
}