using UnityEngine;

namespace Terrain.Foliage
{
    [CreateAssetMenu(fileName = "FoliageSettings", menuName = "Sim/FoliageSettings", order = 2)]
    public class FoliageSettings : ScriptableObject
    {
        [Tooltip("Size of single foliage chunk")]
        public float chunkSize;
    }
}
