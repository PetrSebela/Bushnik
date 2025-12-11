using System;
using System.Collections;
using System.Collections.Generic;
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

        private Dictionary<Vector3, FoliageChunk> _chunks = new();

        public Foliage[] Tests;

        public Mesh BillboardModel;

        public float RenderDistance = 6000;

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
            StartCoroutine(GenerateFoliageChunks());
        }

        IEnumerator GenerateFoliageChunks()
        {
            yield return new WaitForEndOfFrame();
            
            float terrainSize = TerrainManager.Instance.terrainSettings.size;
            int chunkCount = Mathf.CeilToInt(terrainSize / foliageSettings.chunkSize);
            float foliageSize = chunkCount * foliageSettings.chunkSize;

            Vector3 start = new Vector3(-foliageSize / 2, 0, -foliageSize / 2) +
                            new Vector3(foliageSettings.chunkSize / 2, 0, foliageSettings.chunkSize / 2);
            
            List<Vector3> chunkOrigins = new List<Vector3>();
            
            for (int x = 0; x < chunkCount; x++)
            {
                for (int z = 0; z < chunkCount; z++)
                {
                    Vector3 position = start + new Vector3(x, 0, z) * foliageSettings.chunkSize;
                    chunkOrigins.Add(position);
                }
            }

            chunkOrigins.Sort((a, b) => a.sqrMagnitude.CompareTo(b.sqrMagnitude));
            
            Queue<Vector3> chunkOriginsQueue = new(chunkOrigins);
            
            double iterationStart = Time.realtimeSinceStartupAsDouble;
            while (chunkOriginsQueue.Count > 0)
            {
                var origin = chunkOriginsQueue.Dequeue();
                CreateFoliageChunk(origin);

                if (Time.realtimeSinceStartupAsDouble - iterationStart < 0.1f) 
                    continue;
                
                iterationStart = Time.realtimeSinceStartupAsDouble;
                yield return null;
            }
            
            RemovePruned();
        }

        /// <summary>
        /// Creates new foliage chunk
        /// </summary>
        /// <param name="position">origin of created chunk</param>
        void CreateFoliageChunk(Vector3 position)
        {
            GameObject chunk = new GameObject("Chunk");
            var foliageChunk = chunk.AddComponent<FoliageChunk>();
            chunk.transform.SetParent(_foliageParent.transform);
            chunk.transform.localPosition = position;
            foliageChunk.Generate();
            _chunks.Add(new Vector3(position.x, 0, position.z), foliageChunk);
        }

        /// <summary>
        /// Removes inactive chunks
        /// </summary>
        void RemovePruned()
        {
            List<KeyValuePair<Vector3,FoliageChunk>> pruned = new List<KeyValuePair<Vector3,FoliageChunk>>();
            
            foreach (var pair in _chunks)
                if (pair.Value.GetState == LODState.Pruned)
                    pruned.Add(pair);

            float percentage = (float)pruned.Count / _chunks.Count * 100;
            Debug.Log($"Pruning {pruned.Count} chunks ({percentage:F2}%)");
            
            foreach (var key in pruned)
            {
                Destroy(key.Value);
                _chunks.Remove(key.Key);
            }
        }
        
        /// <summary>
        /// Updates foliage chunks
        /// </summary>
        private void Update()
        {
            var target = TerrainManager.Instance.LODTarget.position;
            var flat = new Vector3(target.x, 0, target.z);
            
            foreach (var pair in _chunks)
            {
                var chunk = pair.Value;
                var position = pair.Key;
                var distance = Vector3.Distance(flat, position);
                
                if (distance < 1000)
                {
                    chunk.SetState(LODState.Active);       
                }
                else if (distance < RenderDistance)
                {
                    chunk.SetState(LODState.Reduced);
                }
                else
                {
                    chunk.SetState(LODState.Suspended);
                }

                float cull = Mathf.Clamp01((distance - RenderDistance / 2) / RenderDistance);
                chunk.Render(cull);
            }
        }
    }
}