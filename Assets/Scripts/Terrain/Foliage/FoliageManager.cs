using System;
using UnityEngine;

namespace Terrain.Foliage
{
    /// <summary>
    /// Class responsible for management and generation of foliage
    /// </summary>
    [RequireComponent(typeof(TerrainManager))]
    public class FoliageManager : MonoBehaviour
    {
        /// <summary>
        /// Foliage settings
        /// </summary>
        public FoliageSettings foliageSettings;
        
        /// <summary>
        /// Parent for foliage chunks (just to keep hierarchy clean)
        /// </summary>
        private GameObject _foliageParent;

        /// <summary>
        /// Private singleton instance
        /// </summary>
        private static FoliageManager _instance;

        /// <summary>
        /// Singleton getter
        /// </summary>
        public static FoliageManager Instance => _instance;
        
        public Foliage TestingModel;
        public Mesh BillboardModel;
        
        /// <summary>
        /// Manager setup and singleton init
        /// </summary>
        /// <exception cref="Exception">Exception when there would be two singletons</exception>
        public void Awake()
        {
            _foliageParent = new GameObject("Foliage");
            _foliageParent.transform.SetParent(transform);
            _foliageParent.transform.localPosition = Vector3.zero;

            if (_instance == null)
                _instance = this;
            else
                throw new Exception("Duplicate instance of foliage manager");
        }

        /// <summary>
        /// Generates foliage chunks
        /// </summary>
        /// TODO: If performance on generation is issus (which it will be), convert this into coroutine and split load across multiple frames 
        public void Start()
        {
            return;
            float terrainSize = TerrainManager.Instance.terrainSettings.size;
            int chunkCount = Mathf.CeilToInt(terrainSize / foliageSettings.chunkSize);
            float foliageSize = chunkCount * foliageSettings.chunkSize;

            Vector3 start = new Vector3(-foliageSize / 2, 0, -foliageSize / 2) + new Vector3(foliageSettings.chunkSize/2, 0, foliageSettings.chunkSize/2);

            for (int x = 0; x < chunkCount; x++)
            {
                for (int z = 0; z < chunkCount; z++)
                {
                    Vector3 position = start + new Vector3(x, 0, z) * foliageSettings.chunkSize;
                    GameObject chunk = new GameObject("Chunk", typeof(FoliageChunk));
                    chunk.transform.SetParent(_foliageParent.transform);
                    chunk.transform.localPosition = position;
                }
            }
        }
    }
}