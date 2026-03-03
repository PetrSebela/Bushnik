using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Interests
{
    public class PointOfInterest : MonoBehaviour
    {
        public readonly List<TerrainAffectorData> TerrainAffectors = new();
    }
    
    public struct TerrainAffectorData
    {
        public Vector3 From;
        public Vector3 To;
        public float Width;
    }
}
