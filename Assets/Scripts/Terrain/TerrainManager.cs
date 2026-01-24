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
        /// LOD target 
        /// </summary>
        public Transform LODTarget;

        /// <summary>
        /// Highest LOD
        /// </summary>
        private Chunk _terrainRoot;

        /// <summary>
        /// Tracker chunk position (highest possible LOD)
        /// </summary>
        private Vector3 _chunkPosition = Vector3.zero;

        /// <summary>
        /// If the tree was fragmented in the last frame
        /// </summary>
        private bool _fragmented = true;
        
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
            Debug.Log($"Using {targetDepth} quad-tree layer");
            _terrainRoot = Chunk.GetChunk(Vector3.zero, transform, ComputeProxy.Instance.terrainSettings.size, targetDepth);
            
            _chunkPosition = GetChunkPosition(LODTarget.transform.position);
            _terrainRoot.UpdateLOD(LODTarget.transform.position);
        }

        private Vector3 GetChunkPosition(Vector3 position)
        {
            return new (
                Mathf.Round(position.x / meshSettings.size),
                0,
                Mathf.Round(position.z / meshSettings.size));
        }

        /// <summary>
        /// Updating LODs
        /// </summary>
        private void Update()
        {
            var currentPosition = GetChunkPosition(LODTarget.transform.position);
            if (currentPosition != _chunkPosition)
            {
                _terrainRoot.UpdateLOD(LODTarget.transform.position);
                StaticBatchingUtility.Combine(_terrainRoot.gameObject);
            }
            _chunkPosition = currentPosition;
        }

        public void ForceUpdate(Chunk from)
        {
            from.UpdateLOD(LODTarget.transform.position);
            StaticBatchingUtility.Combine(_terrainRoot.gameObject);
        }
    }
}