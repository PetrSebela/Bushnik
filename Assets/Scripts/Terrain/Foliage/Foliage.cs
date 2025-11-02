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
    }
}
