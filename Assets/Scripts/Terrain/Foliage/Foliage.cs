using System;
using UnityEditor;
using UnityEngine;

namespace Terrain.Foliage
{
    [CreateAssetMenu(fileName = "Foliage", menuName = "Sim/Foliage", order = 3)]
    public class Foliage : ScriptableObject
    {
        public GameObject foliagePrefab;
        public Material billboardMaterial;

        public float minSize = 1f;
        public float maxSize = 1f;
        public float density = 0.001f;
        
        public float maxAngle = 20f;
        public float maxHeight = 1000f;
    }
}
