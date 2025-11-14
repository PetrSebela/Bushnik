using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Terrain.Foliage
{
    public class FoliageChunk : MonoBehaviour
    {
        private LODState _state = LODState.Active;

        private Instances[] _test;
        public void Start()
        {

            var modelCount = FoliageManager.Instance.Tests.Length;
            _test = new Instances[modelCount];

            float area = Mathf.Pow(FoliageManager.Instance.foliageSettings.chunkSize, 2);
            float density = 20f; //Trees per 10000m^2
            int count = Mathf.CeilToInt((area / 10000f) * density / modelCount);


            int totalCount = 0;
            for (int i = 0; i < modelCount; i++)
            {
                Vector3[] samples = new Vector3[count];
                
                for (int j = 0; j < count; j++)
                {
                    Vector3 offset = new Vector3(
                        Random.Range(-1f, 1f),
                        0,
                        Random.Range(-1f, 1f));

                    Vector3 point = transform.position + offset * (FoliageManager.Instance.foliageSettings.chunkSize / 2);
                    samples[j] = point;
                }
                
                var valid = ComputeProxy.Instance.SamplePoints(ref samples);
                totalCount += valid.Length; 
                
                _test[i] = new(FoliageManager.Instance.Tests[i], valid);
            }
         
            if (totalCount == 0)
                _state = LODState.Pruned;
        }
        
        public void Render(float culled)
        {
            if (_state == LODState.Suspended || _state == LODState.Pruned)
                return;
            
            foreach (var instances in _test)
                instances.Render(culled);
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
