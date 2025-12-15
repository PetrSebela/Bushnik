using System;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    /// <summary>
    /// Spreads slow terrain computation across multiple frames to avoid large spikes in frametime
    /// </summary>
    public class LoadBalancer : MonoBehaviour
    {
        /// <summary>
        /// Load balancer instance
        /// </summary>
        private static LoadBalancer _instance;

        /// <summary>
        /// Load balancer accessor
        /// </summary>
        public static LoadBalancer Instance => _instance;

        private Stack<Chunk> _requests = new();
        
        private void Awake()
        {
            _instance = this;
        }

        void FixedUpdate()
        {
            ProcessRequest();
        }

        private void ProcessRequest()
        {
            if(!ComputeProxy.Instance.PipelineClear || _requests.Count == 0)
                return; 
            
            var request = _requests.Pop();
            StartCoroutine(
            ComputeProxy.Instance.GetTerrainMesh(
                request.transform.position, 
                request.Size, 
                request.Depth, 
                request)
            );
        }
        
        public void RegisterRequest(Chunk chunk)
        {
            _requests.Push(chunk);
        }
    }
}
