using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using Utility;

namespace Terrain
{
    /// <summary>
    /// Class representing single quazi parallel workers interfacing with GPU
    /// </summary>
    public class TerrainWorker
    {
        /// <summary>
        /// ID of mesh kernel
        /// </summary>
        private readonly int _terrainMeshKernelID;
        
        /// <summary>
        /// Compute buffer containing mesh vertices
        /// </summary>
        private readonly ComputeBuffer _terrainVertexBuffer;
        
        /// <summary>
        /// Compute buffer containing mesh indices
        /// </summary>
        private readonly ComputeBuffer _terrainIndexBuffer;
        
        /// <summary>
        /// Compute buffer containing terrain normals
        /// </summary>
        private readonly ComputeBuffer _terrainNormalBuffer;
        
        /// <summary>
        /// Compute buffer containing additional terrain data that are passed to the fragment and vertex shaders (use uv0)
        /// </summary>
        private readonly ComputeBuffer _terrainDataBuffer;

        /// <summary>
        /// Number of thread groups to be dispatched
        /// </summary>
        private readonly int _threadGroups;

        /// <summary>
        /// Terrain compute shader
        /// </summary>
        private readonly ComputeShader _shader;
        
        /// <summary>
        /// If worker is free
        /// </summary>
        private bool _isFree = true;
        
        /// <summary>
        /// If worker is free
        /// </summary>
        private bool IsFree => _isFree;

        /// <summary>
        /// Invoked upon finishing each task
        /// </summary>
        public Action<TerrainWorker> OnTaskFinished;

        /// <summary>
        /// Constructor creating single worker instance
        /// </summary>
        /// <param name="meshSettings">Mesh settings</param>
        /// <param name="shader">Terrain compute shader</param>
        public TerrainWorker(MeshSettings meshSettings, ComputeShader shader)
        {
            _terrainMeshKernelID = shader.FindKernel("TerrainMesh");
            _shader = shader;
            _threadGroups = Mathf.CeilToInt(meshSettings.resolution / 32f);
            
            int vertexCount = (int)Mathf.Pow(meshSettings.resolution, 2);
            int indicesCount = (int)Mathf.Pow(meshSettings.resolution - 1, 2) * 6;

            _terrainVertexBuffer = new ComputeBuffer(vertexCount, sizeof(float) * 3);
            _terrainIndexBuffer = new ComputeBuffer(indicesCount, sizeof(uint));
            _terrainNormalBuffer = new ComputeBuffer(vertexCount, sizeof(float) * 3);
            _terrainDataBuffer = new ComputeBuffer(vertexCount, sizeof(float) * 2);
        }

        /// <summary>
        /// Disposes used compute buffers
        /// </summary>
        public void Dispose()
        {
            _terrainVertexBuffer.Dispose();
            _terrainIndexBuffer.Dispose();
            _terrainNormalBuffer.Dispose();
            _terrainDataBuffer.Dispose();
        }

        /// <summary>
        /// Binds local worker buffers
        /// </summary>
        private void BindBuffers()
        {
            _shader.SetBuffer(_terrainMeshKernelID, "VertexBuffer", _terrainVertexBuffer);
            _shader.SetBuffer(_terrainMeshKernelID, "IndexBuffer", _terrainIndexBuffer);
            _shader.SetBuffer(_terrainMeshKernelID, "NormalBuffer", _terrainNormalBuffer);
            _shader.SetBuffer(_terrainMeshKernelID, "DataBuffer", _terrainDataBuffer);
        }
        
        public async Task ComputeMesh(Chunk chunk)
        {
            if (!_isFree)
                throw new Exception("Worker not free");
            
            _isFree = false;
            
            BindBuffers();
            
            _shader.SetFloat("ChunkSize", chunk.Size);
            _shader.SetFloats("ChunkPosition", chunk.transform.position.x, chunk.transform.position.y, chunk.transform.position.z);
            _shader.SetInt("ChunkDepth", chunk.Depth);
            
            _shader.Dispatch(_terrainMeshKernelID, _threadGroups, 1, _threadGroups);
            
            var verticesRequest = GPUUtils.ReadBufferAsync<Vector3>(_terrainVertexBuffer);
            var indicesRequest = GPUUtils.ReadBufferAsync<int>(_terrainIndexBuffer);
            var normalsRequest = GPUUtils.ReadBufferAsync<Vector3>(_terrainNormalBuffer);
            var dataRequest = GPUUtils.ReadBufferAsync<Vector2>(_terrainDataBuffer);
            
            await Task.WhenAll(verticesRequest, indicesRequest, normalsRequest, dataRequest);
            
            Mesh mesh = new()
            {
                vertices = verticesRequest.Result,
                triangles = indicesRequest.Result,
                normals = normalsRequest.Result,
                uv2 = dataRequest.Result
            };
            
            mesh.RecalculateTangents();
            MeshBaker.Instance.Bake(chunk, mesh);
            _isFree = true;
            OnTaskFinished.Invoke(this);
        }
    }
}
