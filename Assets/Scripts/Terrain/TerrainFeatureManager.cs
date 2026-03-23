using System;
using System.Collections.Generic;
using Terrain.Interests;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Terrain
{
    /// <summary>
    /// Class responsible for managing points of interest which affect terrain
    /// </summary>
    public class TerrainFeatureManager : MonoBehaviour
    {
        /// <summary>
        /// Parent of all POIs
        /// </summary>
        public Transform pointOfInterestParent;

        /// <summary>
        /// Array of POI descriptors
        /// </summary>
        [SerializeField] private PointOfInterestDescriptor[] features;
        
        /// <summary>
        /// List of all generated POIs
        /// </summary>
        private readonly List<PointOfInterest> _interests = new();
        
        /// <summary>
        /// POI getter
        /// </summary>
        public List<PointOfInterest> Interests => _interests;
        
        /// <summary>
        /// Singleton instance
        /// </summary>
        private static TerrainFeatureManager _instance;
        
        /// <summary>
        /// Singleton getter
        /// </summary>
        public static TerrainFeatureManager Instance => _instance;
        
        
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
        /// Initialization method
        /// </summary>
        public void Init()
        {
            Random.InitState(TerrainManager.Instance.terrainSettings.seed.GetHashCode());
            foreach (var feature in features)
            {
                var pois = feature.GetPointOfInterest();
                _interests.AddRange(pois);
            }
        }

        /// <summary>
        /// Returns terrain affectors of generates POIs
        /// </summary>
        /// <returns></returns>
        public TerrainAffectorData[] GetAffectors()
        {
            List<TerrainAffectorData> affectors = new();

            foreach (var poi in _interests)
                affectors.AddRange(poi.TerrainAffectors);
            
            return affectors.ToArray();
        }
    }
}
