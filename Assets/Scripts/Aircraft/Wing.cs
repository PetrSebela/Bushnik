using System;
using UnityEngine;

namespace Aircraft
{
    public class Wing : MonoBehaviour
    {
        [SerializeField] private Vector2 size;
        private void OnDrawGizmos()
        {
            var wingSurface = new Mesh();
            
            Vector3[] wingVertices =
            {
                new Vector3(size.x / 2, 0, size.y / 2),
                new Vector3(-size.x / 2, 0, size.y / 2),
                new Vector3(size.x / 2, 0, -size.y / 2),
                new Vector3(-size.x / 2, 0, -size.y / 2),
            };
            int[] surfaceTriangles =
            {
                0, 1, 2, 2, 1, 3,
                0, 2, 1, 1, 2, 3
            };
            
            wingSurface.vertices = wingVertices;
            wingSurface.triangles = surfaceTriangles;
            wingSurface.RecalculateNormals();
            
            
            
            Gizmos.color = new Color(0.4f, 0.9f, 0.4f,0.5f);
            Gizmos.DrawMesh(wingSurface, transform.position, transform.rotation);
        }
    }
}
