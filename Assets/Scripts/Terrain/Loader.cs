using System;
using Terrain.Data;
using Terrain.Foliage;
using UnityEngine;

namespace Terrain
{
    public class Loader : MonoBehaviour
    {
        private bool _isLoaded = false;
        
        public bool IsLoaded => _isLoaded;
        
        private Action<float> _progressCallback;
        
        private static Loader _instance;
        public static Loader Instance => _instance;

        private void Awake()
        {
            _instance = this;
        }

        public void Load(Action<float> progressCallback )
        {
            _progressCallback = progressCallback;
            SeaManager.Instance.Init();
            
            TerrainManager.Instance.enabled = false;
            FoliageManager.Instance.enabled = false;
            
            ComputeProxy.Instance.Init();
            TerrainFeatureManager.Instance.GetRunways(OnRunwaysGenerated);
        }


        private void OnRunwaysGenerated(RunwayData[] runways)
        {
            ComputeProxy.Instance.UpdateTerrainAffectors(runways);
            
            TerrainManager.Instance.enabled = true;
            FoliageManager.Instance.enabled = true;
        }
    }
}
