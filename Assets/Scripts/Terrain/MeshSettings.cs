using UnityEngine;

namespace Terrain
{
    /// <summary>
    /// Collection of terrain mesh settings
    /// </summary>
    [CreateAssetMenu(fileName = "MeshSettings", menuName = "Sim/MeshSettigns", order = 1)]
    public class MeshSettings : ScriptableObject
    {
        [Tooltip("Resolution of chunks")]
        [Range(1,32)]
        public int resolution;
        
        [Tooltip("LOD levels")]
        public float LODLevels;
        
        [Tooltip("Minimum size of LOD chunks (highest detail)")]
        public float size;
    }
}
