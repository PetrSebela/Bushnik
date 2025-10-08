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
    }
}
