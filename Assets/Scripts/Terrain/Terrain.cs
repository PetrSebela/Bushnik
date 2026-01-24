using System;
using UnityEngine;

namespace Terrain
{
    public class Terrain : MonoBehaviour
    {
        private void PrintProgress(float progress)
        {
            Debug.Log(progress);
        }
        
        private void Start()
        {
            Loader.Instance.Load(PrintProgress);
        }
    }
}
