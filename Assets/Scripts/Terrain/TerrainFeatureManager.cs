using System;
using Terrain.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Terrain
{
    public class TerrainFeatureManager : MonoBehaviour
    {
        [SerializeField] private int biomeCount;
        [SerializeField] private int seed = 0;
        public static TerrainFeatureManager Instance => _instance;
        private static TerrainFeatureManager _instance;
        
        /// <summary>
        /// Initialize singleton instance
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                throw new Exception("Duplicate instance of TerrainFeatureManager");
        }
        
        /// <summary>
        /// Returns array of generated biomes
        /// </summary>
        /// <returns>Array of GPU ready data</returns>
        public BiomeGPUData[] GetBiomes()
        {
            Random.InitState(seed);
            BiomeGPUData[] biomes = new BiomeGPUData[biomeCount];

            for (int i = 0; i < biomeCount; i++)
            {
                biomes[i] = new BiomeGPUData()
                {   
                    BiomeID = i,
                    Position = new Vector2(
                        Random.Range(-50000, 50000),
                        Random.Range(-50000, 50000)
                        )
                };
            }
            
            return biomes;
        }
    }
}
