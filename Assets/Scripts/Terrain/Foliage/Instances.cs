using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Terrain.Foliage
{
    public class Instances
    {
        private readonly Mesh _mesh;
        private readonly Material _materials;
        private readonly Matrix4x4[]_matrices;
        
        /// <summary>
        /// Create instances for billboards
        /// </summary>
        /// <param name="foliage"></param>
        /// <param name="points"></param>
        public Instances(Foliage foliage, Vector3[] points)
        {
            _mesh = FoliageManager.Instance.BillboardModel;
            _materials = foliage.billboardMaterial;
            
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
            _matrices = instances;
        }

        public void Render(float culled=0)
        {
            // var count = Mathf.CeilToInt((1 - culled) * _matrices.Length);
            Graphics.DrawMeshInstanced(
                _mesh,
                0,
                _materials,
                _matrices);
        }
    }
}
