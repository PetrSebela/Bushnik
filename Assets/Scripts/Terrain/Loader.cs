using System;
using System.Collections;
using Terrain.Data;
using Terrain.Foliage;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Terrain
{
    public class Loader : MonoBehaviour
    {
        private bool _isLoaded = false;
        
        public bool IsLoaded => _isLoaded;
        
        private Action<float> _progressCallback;
        
        private static Loader _instance;
        public static Loader Instance => _instance;
        
        private int _chunksLoaded = 0;

        [SerializeField] LoadingScreen loadingScreen;
        
        public Action AfterPregeneration; 
        public Action AfterLoading; 
        
        private void Awake()
        {
            _instance = this;
        }

        public void Load()
        {
            loadingScreen.BeginLoading();
            SeaManager.Instance.Init();
            TerrainManager.Instance.Init();
            TerrainManager.Instance.enabled = false;
            FoliageManager.Instance.enabled = false;
            
            loadingScreen.SetMessage("Loading runways");
            ComputeProxy.Instance.Init();
            TerrainFeatureManager.Instance.GetRunways(OnRunwaysGenerated);
        }
        
        private void OnRunwaysGenerated(RunwayData[] runways)
        {
            ComputeProxy.Instance.UpdateTerrainAffectors(runways);
            
            TerrainManager.Instance.enabled = true;
            FoliageManager.Instance.enabled = true;
            
            AfterPregeneration?.Invoke();
            
            StartCoroutine(LoadTerrain());
        }

        private void IncrementProgress()
        {
            _chunksLoaded++;
            float progress = _chunksLoaded / 180f; 
            loadingScreen.SetProgress(progress);
        }

        private IEnumerator LoadTerrain()
        {
            loadingScreen.SetMessage("Loading terrain");
            LoadBalancer.Instance.OnRequestDispatched += IncrementProgress;
            TerrainManager.Instance.ForceLODUpdate();

            while (!LoadBalancer.Instance.AllFinished)
            {
                yield return new WaitUntil(() => LoadBalancer.Instance.AllFinished);
                yield return new WaitForSeconds(1f);
            }

            LoadBalancer.Instance.OnRequestDispatched -= IncrementProgress;
            LoadingFinished();
        }

        private void LoadingFinished()
        {
            AfterLoading?.Invoke();
            _isLoaded = true;
            loadingScreen.FinishLoading();
        }
    }
}
