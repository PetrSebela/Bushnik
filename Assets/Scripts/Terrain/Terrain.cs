using System;
using UnityEngine;

namespace Terrain
{
    public class Terrain : MonoBehaviour
    {
        public Transform player;
        
        private static Terrain _instance;
        public static Terrain Instance => _instance;

        void Awake()
        {
            _instance = this;
        }
        
        void Start()
        {
            // Loader.Instance.Load();
        }
    }
}
