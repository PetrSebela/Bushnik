using System;
using UnityEngine;

namespace Terrain
{
    /// <summary>
    /// Class responsible for generation and management of terrain LODs 
    /// </summary>
    public class TerrainManager : MonoBehaviour
    {
        /// <summary>
        /// Private singleton instance
        /// </summary>
        private static TerrainManager _instance;
        
        /// <summary>
        /// Public singleton getter
        /// </summary>
        public static TerrainManager Instance => _instance;

        /// <summary>
        /// Mesh settings
        /// </summary>
        public MeshSettings meshSettings;

        /// <summary>
        /// Terrain settings
        /// </summary>
        public TerrainSettings terrainSettings;
        
        /// <summary>
        /// Highest LOD
        /// </summary>
        private Chunk _terrainRoot;

        /// <summary>
        /// Tracker chunk position (highest possible LOD)
        /// </summary>
        private Vector3 _chunkPosition = Vector3.zero;
        
        /// <summary>
        /// Singleton initialization
        /// </summary>
        /// <exception cref="Exception">Exception when there would be two singletons</exception>
        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                throw new Exception("Duplicate instance of TerrainManager");
        }

        /// <summary>
        /// Singleton destruction
        /// </summary>
        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        /// <summary>
        /// LOD system setup
        /// </summary>
        private void Start()
        {
            // TODO: decide on material management and move this to propper module
            terrainSettings.material.SetFloat("_ShowHeight", terrainSettings.snowHeight);
            
            int nodeCount = Mathf.CeilToInt(ComputeProxy.Instance.terrainSettings.size / meshSettings.size);
            int targetDepth = (int)Mathf.Log(nodeCount, 2);
            
            _terrainRoot = Chunk.GetChunk(Vector3.zero, transform, ComputeProxy.Instance.terrainSettings.size, targetDepth);
            
            _chunkPosition = GetChunkPosition(Terrain.Instance.player.position);
            _terrainRoot.UpdateLOD(Terrain.Instance.player.position);
        }

        private Vector3 GetChunkPosition(Vector3 position)
        {
            return new (
                Mathf.Round(position.x / meshSettings.size),
                0,
                Mathf.Round(position.z / meshSettings.size));
        }

        public void ForceLODUpdate()
        {
            _terrainRoot.UpdateLOD(Terrain.Instance.player.position);
        }

        /// <summary>
        /// Updating LODs
        /// </summary>
        private void Update()
        {
            var currentPosition = GetChunkPosition(Terrain.Instance.player.position);
            if (currentPosition != _chunkPosition)
            {
                _terrainRoot.UpdateLOD(Terrain.Instance.player.position);
                // StaticBatchingUtility.Combine(_terrainRoot.gameObject);
            }
            _chunkPosition = currentPosition;
        }

        public void ForceUpdate(Chunk from)
        {
            from.UpdateLOD(Terrain.Instance.player.position);
            // StaticBatchingUtility.Combine(_terrainRoot.gameObject);
        }
    }
}