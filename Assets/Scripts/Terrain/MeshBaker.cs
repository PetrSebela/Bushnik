using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;

namespace Terrain
{
    /// <summary>
    /// Class responsible for baking collision mesh in separate thread
    /// </summary>
    public class MeshBaker : MonoBehaviour
    {
        /// <summary>
        /// List where all incoming requests are buffered
        /// </summary>
        private readonly List<ColliderBakeRequest> _requests = new();
        
        private readonly int _batchSize = 16;
        
        private NativeArray<int> _meshIDs;
        
        /// <summary>
        /// Flag for representing if the pipeline is occupied
        /// </summary>
        private bool _pipelineClear = true;
        
        /// <summary>
        /// Private singleton instance
        /// </summary>
        private static MeshBaker _instance;
        
        /// <summary>
        /// Singleton getter
        /// </summary>
        public static MeshBaker Instance => _instance;
        
        void Awake()
        {
            _instance = this;
            _meshIDs = new NativeArray<int>(_batchSize, Allocator.Persistent);
        }

        void OnDestroy()
        {
            _meshIDs.Dispose();
        }

        /// <summary>
        /// Processing the buffered requests
        /// </summary>
        private void Update()
        {
            if(_pipelineClear)
                StartCoroutine(ProcessRequests());
        }

        /// <summary>
        /// Registers request for async baking of collider mesh
        /// The mesh will be set to chunk upon completing the request
        /// </summary>
        /// <param name="chunk">Chunk to which the collider will be assigned</param>
        /// <param name="mesh">Mesh which will be baked and assigned</param>
        public void Bake(Chunk chunk, Mesh mesh)
        {
            var request = new ColliderBakeRequest(chunk, mesh);
            _requests.Add(request);
        }

        /// <summary>
        /// Bakes the mesh
        /// Adaptation of https://docs.unity3d.com/ScriptReference/Physics.BakeMesh.html
        /// </summary>
        private IEnumerator ProcessRequests()
        {
            if (_requests.Count == 0)
                yield break;

            _pipelineClear = false;
            
            var realSize = Mathf.Min(_requests.Count, _batchSize);

            var pending = new List<ColliderBakeRequest>();

            for (int meshIndex = 0; meshIndex < realSize; meshIndex++)
            {
                var request = _requests[0];
                _requests.RemoveAt(0);
                
                _meshIDs[meshIndex] = request.Mesh.GetInstanceID();
                pending.Add(request);
            }
            
            
            
            // Create local copy of requests and clear them so that incoming requests can be buffered
            // var pending = new List<ColliderBakeRequest>(_requests);
            // _requests.Clear();
            
            // Create job params
            // NativeArray<int> meshIDs = new NativeArray<int>(pending.Count, Allocator.TempJob);
            // for (int i = 0; i < pending.Count; i++)
                // meshIDs[i] = pending[i].Mesh.GetInstanceID();

            // Baking
            var job = new ColliderBakeJob(_meshIDs);
            var handle = job.Schedule(realSize, _batchSize);
            yield return new WaitUntil (() => handle.IsCompleted);

            handle.Complete();
            // meshIDs.Dispose();
            
            // Callbacks
            foreach (var request in pending)
                request.Chunk.SetMesh(request.Mesh);
            
            _pipelineClear = true;
        }
    }

    /// <summary>
    /// Struct for tracking bake requests
    /// </summary>
    public struct ColliderBakeRequest
    {
        public readonly Chunk Chunk;
        public readonly Mesh Mesh;

        public ColliderBakeRequest(Chunk chunk, Mesh mesh)
        {
            Chunk = chunk;
            Mesh = mesh;
        }
    }

    /// <summary>
    /// Job struct for asynchronous baking of colliders
    /// Taken from https://docs.unity3d.com/ScriptReference/Physics.BakeMesh.html
    /// </summary>
    public readonly struct ColliderBakeJob : IJobParallelFor
    {
        private readonly NativeArray<int> _meshIDs;

        public ColliderBakeJob(NativeArray<int> meshIDs)
        {
            _meshIDs = meshIDs;
        }

        public void Execute(int index)
        {
            Physics.BakeMesh(_meshIDs[index], false);
        }
    }
}
