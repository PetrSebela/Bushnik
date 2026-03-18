using System.Collections;
using Terrain.Foliage;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace Terrain
{
    /// <summary>
    /// Class responsible for sequencing of actions that are necessary for correct terrain loading
    /// </summary>
    public class Loader : Singleton<Loader>
    {
        /// <summary>
        /// What actions should be performed as part of pipeline initialization
        /// </summary>
        public UnityEvent afterPregeneration;
        
        /// <summary>
        /// What actions should be performed after generation
        /// </summary>
        public UnityEvent afterLoading; 
        
        /// <summary>
        /// Begins pipeline initialization
        /// </summary>
        public void Load()
        {
            SeaManager.Instance.Init();
            TerrainManager.Instance.Init();
            TerrainManager.Instance.enabled = false;
            FoliageManager.Instance.enabled = false;
            
            ComputeProxy.Instance.Init();
            
            TerrainFeatureManager.Instance.Init();
            
            ComputeProxy.Instance.UpdateTerrainAffectors();
            
            TerrainManager.Instance.enabled = true;
            FoliageManager.Instance.enabled = true;
            
            afterPregeneration?.Invoke();
            
            StartCoroutine(LoadTerrain());
        }
        
        /// <summary>
        /// Loads terrain around player
        /// </summary>
        private IEnumerator LoadTerrain()
        {
            TerrainManager.Instance.ForceLODUpdate();

            while (!LoadBalancer.Instance.AllFinished)
            {
                yield return new WaitUntil(() => LoadBalancer.Instance.AllFinished);
                yield return new WaitForSeconds(1f);
            }

            afterLoading?.Invoke();
        }
    }
}
