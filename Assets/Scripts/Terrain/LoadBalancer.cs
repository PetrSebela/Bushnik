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
        /// List of all mesh requests
        /// </summary>
        private readonly List<Chunk> _terrainMeshRequests = new();
        
        /// <summary>
        /// List of all point requests
        /// </summary>
        private readonly List<PointsRequest> _pointRequests = new();
        
        /// <summary>
        /// Singleton init
        /// </summary>
        private void Awake()
        {
            _instance = this;
        }
        
        void LateUpdate()
        {
            ProcessMeshRequest();
            ProcessPointsRequest();
        }

        /// <summary>
        /// Tries to fulfill next mesh request
        /// </summary>
        private void ProcessMeshRequest()
        {
            while (ComputeProxy.Instance.HasFreeWorker && _terrainMeshRequests.Count > 0 )
            {
                var request = _terrainMeshRequests[^1];
                _terrainMeshRequests.RemoveAt(_terrainMeshRequests.Count - 1);
                ComputeProxy.Instance.GetTerrainMesh(request);
            }
        }
        
        private void ProcessPointsRequest()
        {
            if (_pointRequests.Count == 0)
                return;
            
            var request = _pointRequests[0];
            _pointRequests.RemoveAt(0);

            if (request.Foliage == null)
            {
                _ = ComputeProxy.Instance.SamplePoints(
                    request.Points,
                    request.OnRequestComplete);
            }
            else
            {
                _ = ComputeProxy.Instance.SamplePoints(
                    request.Points,
                    request.OnRequestComplete,
                    request.Foliage);
            }
        }
        
        /// <summary>
        /// Adds chunk to request queue
        /// </summary>
        /// <param name="chunk">Requested chunk</param>
        public void RegisterRequest(Chunk chunk)
        {
            _terrainMeshRequests.Add(chunk);
        }

        public void RegisterRequest(PointsRequest request)
        {
            _pointRequests.Add(request);
        }
        
        /// <summary>
        /// Cancels any requests for said chunk 
        /// </summary>
        /// <param name="chunk">Chunk for which the requests will be cancelled</param>
        public void CancelRequest(Chunk chunk)
        {
            _terrainMeshRequests.Remove(chunk);
        }
    }

    public class PointsRequest
    {
        private readonly Vector3[] _points;
        public Vector3[] Points => _points;
        
        private Foliage.Foliage _foliage;
        public Foliage.Foliage Foliage => _foliage;
        
        public Action<Vector3[]> OnRequestComplete;

        public PointsRequest(Vector3[] points)
        {
            _points = points;
        }        
        
        public PointsRequest(Vector3[] points, Foliage.Foliage foliage)
        {
            _points = points;
            _foliage = foliage;
        }
    }
}
