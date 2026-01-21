using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Terrain.Foliage
{
    public class FoliageChunk : MonoBehaviour
    {
        /// <summary>
        /// Current chunk state ( maybe discard later )
        /// </summary>
        private LODState _state = LODState.Active;

        /// <summary>
        /// Array of all foliage instances in this chunk
        /// </summary>
        private Instances[] _instances;
        
        /// <summary>
        /// Generates foliage in chunk
        /// </summary>
        public void Generate()
        {
            var modelCount = FoliageManager.Instance.PlacedObjects.Length;
            _instances = new Instances[modelCount];
            
            float area = Mathf.Pow(FoliageManager.Instance.foliageSettings.chunkSize, 2);
            
            Vector3 minCube = transform.position - Vector3.one * (FoliageManager.Instance.foliageSettings.chunkSize / 2);
            Vector3 maxCube = transform.position + Vector3.one * (FoliageManager.Instance.foliageSettings.chunkSize / 2);
            
            int totalCount = 0;
            for (int i = 0; i < modelCount; i++)
            {
                float density = FoliageManager.Instance.PlacedObjects[i].density; //Trees per 10000m^2
                int count = Mathf.CeilToInt(area * density);
                Vector3[] samples = Utility.RandomProvider.GetRandomPointsIn(minCube, maxCube, count);
                var valid = ComputeProxy.Instance.SamplePoints(ref samples, FoliageManager.Instance.PlacedObjects[i]);
                totalCount += valid.Length;
                _instances[i] = new(FoliageManager.Instance.PlacedObjects[i], valid);
            }
         
            if (totalCount == 0)
                _state = LODState.Pruned;
        }
        
        public void Render()
        {
            if (_state == LODState.Suspended || _state == LODState.Pruned)
                return;
            
            foreach (var instances in _instances)
                instances.Render();
        }

        private void OnDrawGizmos()
        {
            switch (_state)
            {
                case LODState.Active:
                    Gizmos.color = Color.green;
                    break;
                
                case  LODState.Reduced:
                    Gizmos.color = Color.yellow;
                    break;
                
                case  LODState.Suspended:
                    Gizmos.color = Color.red;
                    break;
                
                case  LODState.Pruned:
                    Gizmos.color = Color.cyan;
                    break;
            }
            
            float size = FoliageManager.Instance.foliageSettings.chunkSize;
            Gizmos.DrawWireCube(transform.position, new Vector3(size, size, size));
        }

        public void SetState(LODState state)
        {
            if (_state == LODState.Pruned)
                return;
            
            _state = state;
        }

        public LODState GetState => _state;
    }

    /// <summary>
    /// Represents state of foliage chunk
    /// </summary>
    public enum LODState
    {
        Active,
        Reduced,
        Suspended,
        Pruned
    }
}
