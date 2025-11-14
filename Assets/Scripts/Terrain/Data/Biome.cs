using UnityEngine;

namespace Terrain.Data
{
    /// <summary>
    /// Scriptable object representing combination of terrain type and foliage
    /// </summary>
    [CreateAssetMenu(fileName = "Biome", menuName = "Sim/Biome", order = 1)]
    public class Biome : ScriptableObject
    {
        
    }


    /// <summary>
    /// Enum of all possible terrain types
    /// </summary>
    public enum TerrainType
    {
        
    }

    
    /// <summary>
    /// Struct used for passing relevant information to the compute shader
    /// </summary>
    public struct BiomeGPUData
    {
        // Central position of said biome
        public Vector2 Position;    
        
        // Biome ID, only for debugging purposes
        public int BiomeID;
    }
}
