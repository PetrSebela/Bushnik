using UnityEngine;

namespace Terrain.Foliage
{
    public class Instances
    {
        private readonly Mesh _mesh;
        private readonly Material _materials;
        private readonly Foliage _foliage;
        private Matrix4x4[] _matrices;
        private PointsRequest _request;
        
        /// <summary>
        /// Create instances for billboards
        /// </summary>
        /// <param name="foliage"></param>
        /// <param name="points"></param>
        public Instances(Foliage foliage, Vector3[] points)
        {
            _foliage = foliage;
            _mesh = FoliageManager.Instance.BillboardModel;
            _materials = foliage.billboardMaterial;
            
            PointsRequest request = new PointsRequest(points, foliage);
            request.OnRequestComplete += SetPoints;
            LoadBalancer.Instance.RegisterRequest(request);
        }
        
        /// <summary>
        /// Callback for receiving generated point from GPU
        /// </summary>
        /// <param name="points"></param>
        private void SetPoints(Vector3[] points)
        {
            Matrix4x4[] instances = new Matrix4x4[points.Length];
            for (int sampleIndex = 0; sampleIndex < points.Length; sampleIndex++)
            {
                float scale = Random.Range(_foliage.minSize, _foliage.maxSize);
                float rotation = Random.Range(0, 360);
                Matrix4x4 matrix = Matrix4x4.TRS(
                    points[sampleIndex], 
                    Quaternion.AngleAxis(rotation, Vector3.up), 
                    Vector3.one * scale);
                instances[sampleIndex] = matrix;
            }
            _matrices = instances;
        }

        public void Render()
        {   
            if(_matrices == null)
                return;
            
            Graphics.DrawMeshInstanced(
                _mesh,
                0,
                _materials,
                _matrices);
        }
    }
}
