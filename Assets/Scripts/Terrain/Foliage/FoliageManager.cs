using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Terrain.Foliage
{
    /// <summary>
    /// Class responsible for management and generation of foliage
    /// </summary>
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

        /// <summary>
        /// Generated foliage
        /// </summary>
        public Foliage[] PlacedObjects;
        
        /// <summary>
        /// Mesh used for rendering foliage billboards
        /// </summary>
        public Mesh BillboardModel;

        /// <summary>
        /// Foliage render distance
        /// </summary>
        private float _renderDistance = 6000;
        
        /// <summary>
        /// Render distance getter
        /// </summary>
        public float RenderDistance => _renderDistance;
        
        HashSet<Vector3> existingChunks = new();
        
        Vector3 lastChunkPosition = Vector3.zero;

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


        private HashSet<Vector3> GetGrid(Vector3 position)
        {
            HashSet<Vector3> grid = new();
            
            int chunkRadius = Mathf.CeilToInt(_renderDistance / foliageSettings.chunkSize);

            for (int x = -chunkRadius; x <= chunkRadius; x++)
            {
                for (int z = -chunkRadius; z <= chunkRadius; z++)
                {
                    Vector3 point = (position + new Vector3(x, 0, z)) * foliageSettings.chunkSize; 
                    grid.Add(point);
                }
            }

            return grid;
        }

        void UpdateChunks(HashSet<Vector3> current)
        {
            var delete = existingChunks.Except(current);
            var create = current.Except(existingChunks);
            existingChunks = current;

            foreach (var chunkPosition in delete)
            {
                var instance = _chunks[chunkPosition];
                Destroy(instance.gameObject);
                _chunks.Remove(chunkPosition);
            }
            
            CreateChunks(create);
        }
        
        void Update()
        {
            var target = Terrain.Instance.player.position;
            Vector3 origin = new(
                Mathf.Round(target.x / foliageSettings.chunkSize),
                0,
                Mathf.Round(target.z / foliageSettings.chunkSize)
            );
            
            if (origin != lastChunkPosition)
            {
                lastChunkPosition = origin;
                var active = GetGrid(origin);
                UpdateChunks(active);
            }
            
            foreach (var chunk in _chunks.Values)
                chunk.Render();
        }

        void CreateChunks(IEnumerable<Vector3> create)
        {
            foreach (var chunkPosition in create)
            {
                var foliageChunk = FoliageChunk.CreateChunk(chunkPosition, _foliageParent.transform);
                _chunks.Add(chunkPosition, foliageChunk);
            }
        }
        
        
        /// <summary>
        /// Generates foliage chunks
        /// </summary>
        /// TODO: If performance on generation is issus (which it will be), convert this into coroutine and split load across multiple frames 
        public void Start()
        {
            Vector3 origin = new(
                Mathf.Round(Terrain.Instance.player.position.x / foliageSettings.chunkSize),
                0,
                Mathf.Round(Terrain.Instance.player.position.z / foliageSettings.chunkSize)
            );
            lastChunkPosition = origin;
            var active = GetGrid(origin);
            UpdateChunks(active);
            
            SetRenderDistance(_renderDistance);
        }

        /// <summary>
        /// Updates render distance in foliage
        /// </summary>
        /// <param name="distance"> Desired render distance</param>
        public void SetRenderDistance(float distance)
        {
            _renderDistance = distance;
            foreach (var model in PlacedObjects)
            {
                model.billboardMaterial.SetFloat("_RenderDistance", distance);
                model.billboardMaterial.SetFloat("_FadeDistance", distance * 0.1f);
            }
        }
    }
}