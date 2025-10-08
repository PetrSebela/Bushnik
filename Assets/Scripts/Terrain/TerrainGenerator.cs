using UnityEngine;
using System;
using Unity.Mathematics;

namespace Terrain
{
    public class TerrainGenerator : MonoBehaviour
    {
        private static TerrainGenerator _instance;
        public static TerrainGenerator Instance => _instance;

        [SerializeField] private ComputeShader terrainComputeShader;

        private int _terrainMeshKernel;
        private ComputeBuffer _terrainVertexBuffer;
        private ComputeBuffer _terrainIndexBuffer;
        private ComputeBuffer _terrainNormalBuffer;
        private ComputeBuffer _terrainDataBuffer;
        
        public MeshSettings meshSettings;
        
        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                throw new Exception("Duplicate instance of TerrainManager");
        }

        private void Start()
        {
            _terrainMeshKernel = terrainComputeShader.FindKernel("TerrainMesh");
            
            int vertexCount = (int)Mathf.Pow(meshSettings.resolution, 2);
            int indicesCount = (int)Mathf.Pow(meshSettings.resolution - 1, 2) * 6;
            
            _terrainVertexBuffer = new ComputeBuffer(vertexCount, sizeof(float) * 3);
            _terrainIndexBuffer = new ComputeBuffer(indicesCount, sizeof(uint));
            _terrainNormalBuffer = new ComputeBuffer(vertexCount, sizeof(float) * 3);
            _terrainDataBuffer = new ComputeBuffer(vertexCount, sizeof(float) * 2);
            
            terrainComputeShader.SetBuffer(_terrainMeshKernel, "VertexBuffer", _terrainVertexBuffer);
            terrainComputeShader.SetBuffer(_terrainMeshKernel, "IndexBuffer", _terrainIndexBuffer);
            terrainComputeShader.SetBuffer(_terrainMeshKernel, "NormalBuffer", _terrainNormalBuffer);
            terrainComputeShader.SetBuffer(_terrainMeshKernel, "DataBuffer", _terrainDataBuffer);
        }

        private void OnDestroy()
        {
            _terrainVertexBuffer?.Dispose();
            _terrainIndexBuffer?.Dispose();
            _terrainNormalBuffer?.Dispose();
            _terrainDataBuffer?.Dispose();
        }

        public Mesh GetTerrainMesh(Vector3 position, float size, int depth)
        {
            terrainComputeShader.SetFloat("ChunkSize", size);
            terrainComputeShader.SetFloats("ChunkPosition", position.x, position.y, position.z);
            terrainComputeShader.SetInt("ChunkDepth", depth);

            terrainComputeShader.Dispatch(_terrainMeshKernel,1,1,1);

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
            mesh.RecalculateNormals();

            return mesh;

        }
    }
}
