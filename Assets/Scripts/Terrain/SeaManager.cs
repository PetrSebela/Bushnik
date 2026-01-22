using System;
using UnityEngine;

namespace Terrain
{
    public class SeaManager : MonoBehaviour
    {
        private static SeaManager _instance;
        public static SeaManager Instance => _instance;

        private void Awake()
        {
            _instance = this;
        }

        /// <summary>
        /// Scales the terrain to the appropriate size
        /// </summary>
        public void Init()
        {
            transform.localScale = new Vector3(0.1f, 1, 0.1f) * TerrainManager.Instance.terrainSettings.size;
        }
    }
}
