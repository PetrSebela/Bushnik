using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Terrain.Foliage
{
    public class Instances
    {
        private readonly Mesh[] _meshes;
        private readonly Material[] _materials;
        private readonly Matrix4x4[][] _matrices;

        /// <summary>
        /// Create instances for billboards
        /// </summary>
        /// <param name="foliage"></param>
        /// <param name="points"></param>
        public Instances(Foliage foliage, Vector3[] points)
        {
            _meshes = new Mesh[] { FoliageManager.Instance.BillboardModel };
            _materials = new Material[] { foliage.billboardMaterial };
            
            Matrix4x4[] instances = new Matrix4x4[points.Length];
            for (int sampleIndex = 0; sampleIndex < points.Length; sampleIndex++)
            {
                float scale = Random.Range(2f, 3f);
                float rotation = Random.Range(0, 360);
                Matrix4x4 matrix = Matrix4x4.TRS(
                    points[sampleIndex], 
                    Quaternion.AngleAxis(rotation, Vector3.up), 
                    Vector3.one * foliage.billboardSize * scale);
                instances[sampleIndex] = matrix;
            }
            _matrices = new Matrix4x4[1][];
            _matrices[0] = instances;
        }
        
        /// <summary>
        /// Create instances for models
        /// </summary>
        /// <param name="model"></param>
        /// <param name="points"></param>
        public Instances(GameObject model, Vector3[] points)
        {
            var filters = model.GetComponentsInChildren<MeshFilter>();
            var renderers = model.GetComponentsInChildren<MeshRenderer>();
            
            _meshes = new Mesh[filters.Length];
            _materials = new Material[renderers.Length];
            _matrices = new Matrix4x4[filters.Length][];
            
            for (int i = 0; i < renderers.Length; i++)
            {
                _meshes[i] = filters[i].sharedMesh;
                _materials[i] = renderers[i].sharedMaterial;
                
                var offset = renderers[i].gameObject.transform.localPosition;
                var scale = renderers[i].gameObject.transform.lossyScale;
                
                Matrix4x4[] instances = new Matrix4x4[points.Length];
                for (int sampleIndex = 0; sampleIndex < points.Length; sampleIndex++)
                {
                    Matrix4x4 matrix = Matrix4x4.TRS(points[sampleIndex] + offset, Quaternion.identity, scale);
                    instances[sampleIndex] = matrix;
                }
                _matrices[i] = instances;
            }
        }

        public void Render(float culled=0)
        {
            for (int i = 0; i < _meshes.Length; i++)
            {
                var instances = _matrices[i];
                var count = Mathf.CeilToInt((1 - culled) * instances.Length);
                Matrix4x4[] left = new Matrix4x4[count];
                
                System.Array.Copy(instances,0,left,0,count);
                // for (int j = 0; j < count; j++)
                    // left[j] = instances[j];
                
                Graphics.DrawMeshInstanced(
                    _meshes[i],
                    0,
                    _materials[i],
                    left);
            }    
        }
    }
}
