using System;
using UnityEngine;
using Utility;

namespace Aircraft.Components
{   
    /// <summary>
    /// Basic suspension (and tire) model TODO: split into separate models
    /// </summary>
    public class Shock : MonoBehaviour
    {
        /// <summary>
        /// Mask with all layers that should be handled as ground
        /// </summary>
        [SerializeField] private LayerMask groundMask;
        
        /// <summary>
        /// Radius of attached wheel
        /// </summary>
        [SerializeField] private float wheelRadius;
        
        /// <summary>
        /// Suspension travel
        /// </summary>
        [SerializeField] private float travel;
        
        /// <summary>
        /// Spring rate (maximum spring force)
        /// </summary>
        [SerializeField] private float springRate;
        
        /// <summary>
        /// Damper coefficient
        /// </summary>
        [SerializeField] private float damping;
        
        /// <summary>
        /// Tire grip coefficient
        /// </summary>
        [SerializeField] private float tireGrip;
        
        /// <summary>
        /// Tire drag coefficient
        /// </summary>
        [SerializeField] private float drag;
        
        /// <summary>
        /// Maximum brake force
        /// </summary>
        [SerializeField] private float maxBrakeForce;

        /// <summary>
        /// Brake input percentage
        /// </summary>
        private float _brakeInput = 0;
        
        /// <summary>
        /// Aircraft body to which all forces will be applied
        /// </summary>
        private Rigidbody _aircraftBody;
        
        
        void Awake()
        {
            _aircraftBody = GetComponentInParent<Rigidbody>();
        }
        
        /// <summary>
        /// Spring origin in world coordinates
        /// </summary>
        private Vector3 ShockOrigin => transform.position + transform.up * travel;
        
        /// <summary>
        /// Spring direction in world space
        /// </summary>
        private Vector3 ShockDirection => transform.up;

        /// <summary>
        /// Sets brake input
        /// </summary>
        /// <param name="value">Brake input value</param>
        public void SetBrakeInput(float value)
        {
            _brakeInput = value;
        }
        
        /// <summary>
        /// Computes spring forces acting in the suspension
        /// Inspired by https://www.youtube.com/watch?v=CdPYlj5uZeI
        /// </summary>
        /// <param name="hit">Place where the attached tire touches the ground</param>
        /// <returns>Force vector</returns>
        private Vector3 GetShockForce(RaycastHit hit)
        {
            var compression = Mathf.Pow(travel - hit.distance,1.5f);
            var velocity = _aircraftBody.GetPointVelocity(ShockOrigin);
            var shockVelocity = Vector3.Dot(ShockDirection, velocity);
            var springForce = compression * springRate - shockVelocity * damping;

            if (springForce <= 0)
                return Vector3.zero;
            
            return hit.normal * springForce;
        }
        
        /// <summary>
        /// Computes tire lateral grip
        /// Inspired by https://www.youtube.com/watch?v=CdPYlj5uZeI
        /// </summary>
        /// <param name="hit">Place where the attached tire touches the ground</param>
        /// <returns>Force vector</returns>
        private Vector3 GetLateralGrip(RaycastHit hit)
        {
            var lateralDirection = Vector3.ProjectOnPlane(transform.right, hit.normal).normalized;
            var velocity = _aircraftBody.GetPointVelocity(ShockOrigin);
            var lateralComponent = Vector3.Dot(lateralDirection, velocity);
            var gripForce = (-lateralComponent * tireGrip) / Time.fixedDeltaTime;
            return lateralDirection * gripForce;
        }
        
        /// <summary>
        /// Computes the tire drag and brakes
        /// </summary>
        /// <param name="hit">Place where the attached tire touches the ground</param>
        /// <returns>Force vector</returns>
        private Vector3 GetDrag(RaycastHit hit)
        {
            var forward = Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized;
            var worldVelocity = _aircraftBody.GetPointVelocity(ShockOrigin);
            var velocity = Vector3.Dot(forward, worldVelocity);
            var direction = Mathf.Sign(velocity);
            var deltaVelocity = Mathf.Abs(velocity);
            var progress = Mathf.Clamp01(deltaVelocity / 5f);
            return forward * (-direction * (drag * progress + maxBrakeForce * _brakeInput));
        }

        /// <summary>
        /// Checks where attached tire touches the ground (or not)
        /// </summary>
        /// <returns>Wheel position in world space</returns>
        public Vector3 GetWheelPosition()
        {
            if (!Physics.SphereCast(ShockOrigin, wheelRadius, -ShockDirection, out RaycastHit hit, travel, groundMask))
                return ShockOrigin - ShockDirection * travel;

            return ShockOrigin - ShockDirection * hit.distance;
        }
        
        /// <summary>
        /// Apply suspension forces
        /// </summary>
        void FixedUpdate()
        {
            if (!Physics.SphereCast(ShockOrigin, wheelRadius, -ShockDirection, out RaycastHit hit, travel, groundMask))
                return;
            
            var spring = GetShockForce(hit);
            var grip = GetLateralGrip(hit);
            var drag = GetDrag(hit);
            var sum = spring + grip + drag;
            _aircraftBody.AddForceAtPosition(sum, hit.point);
        }

        /// <summary>
        /// Draw debug gizmos
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) 
                return;

            if (!Physics.SphereCast(ShockOrigin, wheelRadius, -ShockDirection, out RaycastHit hit, travel))
                return;
            
            Gizmos.color = Color.blue;
            var spring = GetShockForce(hit);
            Gizmos.DrawLine(hit.point, hit.point + spring);
            
            Gizmos.color = Color.yellow;
            var grip = GetLateralGrip(hit);
            Gizmos.DrawLine(hit.point, hit.point + grip);
            
            Gizmos.color = Color.red;
            var drag = GetDrag(hit);
            Gizmos.DrawLine(hit.point, hit.point + drag);
        }
    }
}
