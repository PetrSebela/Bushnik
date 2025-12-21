using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// List of all requests
        /// </summary>
        private List<Chunk> _requests = new();
        
        /// <summary>
        /// Singleton init
        /// </summary>
        private void Awake()
        {
            _instance = this;
        }
        
        void LateUpdate()
        {
            ProcessRequest();
        }

        /// <summary>
        /// Tries to fulfill next request
        /// </summary>
        private void ProcessRequest()
        {
            if(!ComputeProxy.Instance.PipelineClear || _requests.Count == 0)
                return; 
            
            var request = _requests[^1];
            _requests.RemoveAt(_requests.Count - 1);

            _ = ComputeProxy.Instance.GetTerrainMesh(
                request.transform.position,
                request.Size,
                request.Depth,
                request);
        }
        
        /// <summary>
        /// Adds chunk to request queue
        /// </summary>
        /// <param name="chunk">Requested chunk</param>
        public void RegisterRequest(Chunk chunk)
        {
            _requests.Add(chunk);
        }

        /// <summary>
        /// Cancels any requests for said chunk 
        /// </summary>
        /// <param name="chunk">Chunk for which the requests will be cancelled</param>
        public void CancelRequest(Chunk chunk)
        {
            _requests.Remove(chunk);
        }
    }
}
