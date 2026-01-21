using UnityEngine;

namespace Terrain
{
    /// <summary>
    /// Collection of terrain settings
    /// </summary>
    [CreateAssetMenu(fileName = "TerrainSettings", menuName = "Sim/TerrainSettings", order = 1)]
    public class TerrainSettings : ScriptableObject
    {
        [Tooltip("Size of generated terrain")]
        public float size;
        
        [Tooltip("Material used for terrain rendering")]
        public Material material;

        [Tooltip("Maximum terrain height")] 
        public float height;

        [Tooltip("Snow height")] 
        public float snowHeight;

        public int noiseLayers = 20;
        public float baseNoiseFrequency = 0.00005f;
        public float frequencyDecay = 1.35f;
        public float amplitudeDecay = 0.65f;
    }
}
