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

        private Instances _test;
        public void Start()
        {
            // float distance = transform.position.magnitude;
            
            // if(distance < 500)
            //     _state = LODState.Active;
            // else if (distance < 2000)
            //     _state = LODState.Reduced;
            // else if (distance < 5000)
            //     _state = LODState.Suspended;
            // else
            //     _state = LODState.Pruned;

            int count = 1024 * 8 * 4;
            Vector3[] samples = new Vector3[count];
            
            for (int i = 0; i < count; i++)
            {
                Vector3 offset = new Vector3(
                    Random.Range(-1f, 1f),
                    0,
                    Random.Range(-1f, 1f));

                Vector3 point = transform.position + offset * (FoliageManager.Instance.foliageSettings.chunkSize / 2);
                samples[i] = point;
            }
            
            var validSamples = TerrainGenerator.Instance.SamplePoints(ref samples);

            _test = new(FoliageManager.Instance.TestingModel, validSamples);
        }

        public void Update()
        {
            _test.Render();
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
            
            // if(_samples == null)
            //     return;
            //
            // Gizmos.color = Color.magenta;
            // foreach(var point in _samples)
            //     Gizmos.DrawSphere(point, 10f);
        }
    }

    /// <summary>
    /// Represents state of foliage chunk
    /// </summary>
    enum LODState
    {
        Active,
        Reduced,
        Suspended,
        Pruned
    }
}
