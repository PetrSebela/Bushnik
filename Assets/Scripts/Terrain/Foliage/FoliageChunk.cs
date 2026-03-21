using UnityEngine;

namespace Terrain.Foliage
{
    public class FoliageChunk : MonoBehaviour
    {
        /// <summary>
        /// Array of all foliage instances in this chunk
        /// </summary>
        private Instances[] _instances;
        
        private Bounds _bounds;
        public Bounds Bounds => _bounds;

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
            _bounds = new Bounds(position, new Vector3(FoliageManager.Instance.foliageSettings.chunkSize, TerrainManager.Instance.terrainSettings.height, FoliageManager.Instance.foliageSettings.chunkSize));
            _instances = new Instances[modelCount];
            
            float area = Mathf.Pow(FoliageManager.Instance.foliageSettings.chunkSize, 2);
            
            for (int i = 0; i < modelCount; i++)
            {
                float density = FoliageManager.Instance.PlacedObjects[i].density;
                int count = Mathf.CeilToInt(area * density);
                Vector3[] samples = Utility.RandomProvider.GetRandomPointsIn(position, FoliageManager.Instance.foliageSettings.chunkSize, count);
                
                _instances[i] = new(FoliageManager.Instance.PlacedObjects[i], samples);
            }
        }
        
        public void Render()
        {
            foreach (var instances in _instances)
                instances.Render();
        }
    }
}
