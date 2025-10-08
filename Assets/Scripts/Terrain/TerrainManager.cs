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
            int nodeCount = Mathf.CeilToInt(terrainSettings.size / meshSettings.size);
            int targetDepth = (int)Mathf.Log(nodeCount, 2);
            _terrainRoot = Chunk.GetChunk(Vector3.zero, transform, terrainSettings.size, targetDepth);
        }

        /// <summary>
        /// Updating LODs
        /// </summary>
        private void Update()
        {
            _terrainRoot.UpdateLOD(LODTarget.transform.position);
        }
    }
}