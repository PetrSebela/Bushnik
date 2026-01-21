using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    /// <summary>
    /// Chunk representing node of LOD tree
    /// </summary>
    public class Chunk : MonoBehaviour
    {
        /// <summary>
        /// Offset directions of children in LOD tree relative to their parent 
        /// </summary>
        private static readonly Vector3[] OffsetDirections = {
            new (1,0,1),
            new (1,0,-1),
            new (-1,0,1),
            new (-1,0,-1)
        };

        /// <summary>
        /// Size of the chunk
        /// </summary>
        private float _size;

        /// <summary>
        /// Size accessor
        /// </summary>
        public float Size => _size;
        
        /// <summary>
        /// Depth of the chunk inside LOD tree
        /// </summary>
        private int _depth;

        /// <summary>
        /// Depth accessor
        /// </summary>
        public int Depth => _depth;
        
        /// <summary>
        /// Flag representing if chunk has higher LOD available
        /// </summary>
        private bool _fragmented = false;
        
        /// <summary>
        /// Children containing higher LOD
        /// </summary>
        private Chunk[] _children =  new Chunk[4];
        
        /// <summary>
        /// Flag representing upper levels of LOD tree that should not be rendered
        /// </summary>
        private bool _forced = false;
        
        /// <summary>
        /// If chunk has valid mesh
        /// </summary>
        private bool _isReady = false;

        /// <summary>
        /// Is Ready accessor
        /// </summary>
        private bool IsReady => _isReady;

        private bool _canDisable = false;
        
        /// <summary>
        /// Parent chunk
        /// </summary>
        private Chunk _parent;
        
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private MeshCollider _collider;
        
        /// <summary>
        /// Creates and initializes chunk instance
        /// TODO: use prefab instancing
        /// </summary>
        /// <param name="offset">Chunk offset inside parent</param>
        /// <param name="parent">Parent under which the chunk will be created</param>
        /// <param name="size">Size of created chunk</param>
        /// <param name="depth">Depth of the chunk inside LOD tree</param>
        /// <returns></returns>
        public static Chunk GetChunk(Vector3 offset, Transform parent, float size, int depth)
        {
            var gameObject = new GameObject(depth.ToString());
            gameObject.transform.parent = parent;
            gameObject.transform.localPosition = offset;
            var chunk = gameObject.AddComponent<Chunk>();
            var parentChunk = parent.GetComponent<Chunk>();
            chunk.Init(size, depth, parentChunk);
            return chunk;
        }

        /// <summary>
        /// Initializes chunk ( cannot use constructors with MonoBehaviour )
        /// </summary>
        /// <param name="size"> Size of the chunk </param>
        /// <param name="depth"> Depth of the chunk in LOD tree </param>
        /// <param name="parent"> Parent chunk instance </param>
        void Init(float size, int depth, Chunk parent)
        {
            _size = size;
            _depth = depth;
            _parent = parent;

            if (_depth > TerrainManager.Instance.meshSettings.LODLevels)
                return;
            
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _meshRenderer.material = ComputeProxy.Instance.terrainSettings.material;

            if (_depth == 0 || _depth == TerrainManager.Instance.meshSettings.LODLevels)
                _collider = gameObject.AddComponent<MeshCollider>();
            
            LoadBalancer.Instance.RegisterRequest(this);
        }

        /// <summary>
        /// Triggers check if this chunk can be safely disabled
        /// </summary>
        private void NotifyReady()
        {
            _canDisable = true;

            foreach (var child in _children)
                _canDisable &= child.IsReady;
        }

        /// <summary>
        /// Cancels potential request of disabled chunks
        /// </summary>
        private void OnDisable()
        {
            LoadBalancer.Instance.CancelRequest(this);
        }

        /// <summary>
        /// Sets chunk mesh
        /// </summary>
        /// <param name="mesh"> Mesh to be set </param>
        public void SetMesh(Mesh mesh)
        {
            _meshFilter.sharedMesh = mesh;
            _isReady = true;
            
            if (_collider)
                _collider.sharedMesh = mesh;
            
            _parent.NotifyReady();
        }
        
        /// <summary>
        /// Creates higher LOD inside the tree
        /// </summary>
        /// <returns>
        /// If chunk was fragmented
        /// </returns>
        bool Fragment()
        {
            if(_fragmented)
                return false;
            
            _fragmented = true;
            
            for (int i = 0; i < 4; i++)
            {
                var offset =  OffsetDirections[i] * _size / 4;
                _children[i] = GetChunk(offset, transform, _size / 2, _depth - 1);
            }

            return true;
        }
        
        /// <summary>
        /// Updates LOD tree structure so that the highest LOD is rendered close to the 'position'
        /// TODO: limit highest chunk resolution based on target velocity
        /// </summary>
        /// <param name="position"> Target LOD position </param>
        public void UpdateLOD(Vector3 position)
        {
            // TODO: use in shaders to cull unused LODs
            if(_depth <= 0)
                return;

            if (_depth > TerrainManager.Instance.meshSettings.LODLevels)
            {
                Fragment();
                _forced = true;
                _canDisable = true;
            }

            Vector3 flatPosition = new Vector3(position.x, 0, position.z);
            if (Vector3.SqrMagnitude(flatPosition - transform.position) > Mathf.Pow(_size, 2) && !_forced)
            {
                EnableTerrain();
                foreach(var child in _children)
                    child?.gameObject.SetActive(false);
                return;
            }

            Fragment();
            
            if(!_canDisable)
                return;
            
            DisableTerrain();
            
            foreach (var child in _children)
            {
                child.gameObject.SetActive(true);
                child.UpdateLOD(position);
            }
        }
        
        /// <summary>
        /// Visualizes LOD tree structure
        /// </summary>
        void OnDrawGizmos()
        {
            if(!gameObject.activeSelf || _forced)
                return;
            
            float r = _depth * 1233123876 % 255f / 255f * 0.75f;
            float g = _depth * 75340123123 % 255f / 255f * 0.75f;
            float b = _depth * 820002012312 % 255f / 255f * 0.75f;

            Gizmos.color = new Color(r, g, b, 0.25f);
            Gizmos.DrawCube(transform.position + Vector3.up * _depth, new Vector3(_size,1f/ (_depth + 1),_size));
            
            Gizmos.color = new Color(r, g, b, 0.75f);
            Gizmos.DrawWireCube(transform.position + Vector3.up * _depth, new Vector3(_size,1f/ (_depth + 1), _size));
        }


        /// <summary>
        /// Enable terrain mesh rendering
        /// </summary>
        private void EnableTerrain()
        {
            if(!_meshRenderer)
                return;
            
            _meshRenderer.enabled = true;
        }

        /// <summary>
        /// Disables terrain mesh rendering
        /// </summary>
        private void DisableTerrain()
        {            
            if(!_meshRenderer)
                return;
            
            _meshRenderer.enabled = false;
        }
    }
}
