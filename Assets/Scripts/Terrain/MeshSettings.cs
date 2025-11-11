using System;
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
        [Range(1, 128)]
        public int resolution;
        
        [Tooltip("LOD levels")]
        public float LODLevels;
        
        [Tooltip("Minimum size of LOD chunks (highest detail)")]
        public float size;

        private void OnValidate()
        {
            if (resolution % 2 != 1)
                throw new Exception("Mesh resolution must be even");
        }
    }
}
