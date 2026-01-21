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
        // private LODState _state = LODState.Active;

        /// <summary>
        /// Array of all foliage instances in this chunk
        /// </summary>
        private Instances[] _instances;

        public static FoliageChunk CreateChunk(Vector3 position, Transform parent)
        {
            GameObject chunk = new GameObject("Foliage chunk");
            var foliageChunk = chunk.AddComponent<FoliageChunk>();
            foliageChunk.Generate(position);
            chunk.transform.SetParent(parent);
            chunk.transform.localPosition = position;
            return foliageChunk;
        }
        
        /// <summary>
        /// Generates foliage in chunk
        /// </summary>
        private void Generate(Vector3 position)
        {
            var modelCount = FoliageManager.Instance.PlacedObjects.Length;
            _instances = new Instances[modelCount];
            
            float area = Mathf.Pow(FoliageManager.Instance.foliageSettings.chunkSize, 2);
            
            Vector3 minCube = position - Vector3.one * (FoliageManager.Instance.foliageSettings.chunkSize / 2);
            Vector3 maxCube = position + Vector3.one * (FoliageManager.Instance.foliageSettings.chunkSize / 2);
            
            // int totalCount = 0;
            for (int i = 0; i < modelCount; i++)
            {
                float density = FoliageManager.Instance.PlacedObjects[i].density;
                int count = Mathf.CeilToInt(area * density);
                Vector3[] samples = Utility.RandomProvider.GetRandomPointsIn(minCube, maxCube, count);
       
                // var valid = ComputeProxy.Instance.SamplePoints(ref samples, FoliageManager.Instance.PlacedObjects[i]);
                // totalCount += valid.Length;
                _instances[i] = new(FoliageManager.Instance.PlacedObjects[i], samples);
            }
         
            // if (totalCount == 0)
                // _state = LODState.Pruned;
        }
        
        public void Render()
        {
            // if (_state == LODState.Suspended || _state == LODState.Pruned)
            //     return;
            //
            foreach (var instances in _instances)
                instances.Render();
        }

        private void OnDrawGizmos()
        {
            // switch (_state)
            // {
            //     case LODState.Active:
            //         Gizmos.color = Color.green;
            //         break;
            //     
            //     case  LODState.Reduced:
            //         Gizmos.color = Color.yellow;
            //         break;
            //     
            //     case  LODState.Suspended:
            //         Gizmos.color = Color.red;
            //         break;
            //     
            //     case  LODState.Pruned:
            //         Gizmos.color = Color.cyan;
            //         break;
            // }

            Gizmos.color = Color.white;
            float size = FoliageManager.Instance.foliageSettings.chunkSize;
            Gizmos.DrawCube(transform.position, new Vector3(size, size, size));
            Gizmos.DrawWireCube(transform.position, new Vector3(size, size, size));
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
