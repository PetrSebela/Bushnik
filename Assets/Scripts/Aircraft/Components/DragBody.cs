using System;
using UnityEngine;

namespace Aircraft.Components
{
    /// <summary>
    /// Component modeling the drag for a rectangle
    /// </summary>
    public class DragBody : MonoBehaviour
    {
        /// <summary>
        /// Width of modeled ractangle
        /// </summary>
        [SerializeField] private float width;
        
        /// <summary>
        /// Height of modeled reactangle
        /// </summary>
        [SerializeField] private float height;
        
        /// <summary>
        /// Length of modeled rectangle
        /// </summary>
        [SerializeField] private float length;
        
        
        /// <summary>
        /// Aircraft body
        /// </summary>
        private Rigidbody _body;
        
        void Awake()
        {
            _body = Utility.Generic.LocateObjectTowardsRoot<Rigidbody>(transform.parent);
        }
        
        /// <summary>
        /// Computed drag acting to face defined by normal and area
        /// </summary>
        /// <param name="normal">Normal defining the face</param>
        /// <param name="area">Area of the surface</param>
        /// <returns>Magnitude of the drag vector acting along the normal</returns>
        float GetDragInDirection(Vector3 normal, float area)
        {
            var velocity = _body.GetPointVelocity(transform.position);
            var dragCoefficient = Vector3.Dot(normal.normalized,velocity);
            var density = Atmosphere.GetDensityAtPoint(transform.position);
            return -dragCoefficient * density * velocity.magnitude * area / 25f;
        }

        /// <summary>
        /// Applies the drag
        /// </summary>
        private void FixedUpdate()
        {
            var rightArea = length * height;
            var rightDrag = GetDragInDirection(transform.right, rightArea);
            
            var frontArea =  width * height;
            var frontDrag = GetDragInDirection(transform.forward, frontArea);
            
            var upArea = length * width;
            var upDrag = GetDragInDirection(transform.up, upArea);
            
            var drag = transform.right * rightDrag + transform.forward * frontDrag + transform.up * upDrag;
            _body.AddForceAtPosition(drag, transform.position);
        }

        /// <summary>
        /// Draw helpful debug gizmos
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            
            Gizmos.color = new Color(1,0.25f,0.25f,0.5f);
            Gizmos.DrawCube(Vector3.zero, new Vector3(width, height, length));
            
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(width, height, length));


            if (!Application.isPlaying)
                return;

            Gizmos.matrix = Matrix4x4.identity;            
            Gizmos.color = Color.red;
            
            var rightArea = length * height;
            var rightDrag = GetDragInDirection(transform.right, rightArea);
            Gizmos.DrawLine(transform.position, transform.position + transform.right * rightDrag);
            
            var frontArea =  width * height;
            var frontDrag = GetDragInDirection(transform.forward, frontArea);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * frontDrag);
            
            var upArea = length * width;
            var upDrag = GetDragInDirection(transform.up, upArea);
            Gizmos.DrawLine(transform.position, transform.position + transform.up * upDrag);
        }
    }
}
