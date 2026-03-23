using System;
using UnityEditor;
using UnityEngine;

namespace Aircraft.Components
{
    /// <summary>
    /// Basic aircraft engine model
    /// </summary>
    public class Engine : MonoBehaviour
    {
        /// <summary>
        /// Rigidbody to which the forces will be applied
        /// </summary>
        [SerializeField] private Rigidbody _rigidbody;
        
        /// <summary>
        /// Maximum engine thrust
        /// </summary>
        [SerializeField] private float MaxForce;
        
        /// <summary>
        /// Engine idle rpm
        /// </summary>
        [SerializeField] private float idle;
        
        /// <summary>
        /// Max engine rpm
        /// </summary>
        [SerializeField] private float redline;
        
        /// <summary>
        /// Throttle input
        /// </summary>
        private float _throttle = 0;

        /// <summary>
        /// Engine RPM
        /// </summary>
        public float EngineSpeed => idle + _throttle * (redline - idle);
        
        public float Throttle => _throttle;

        public float CrankshaftAngularVelocity => EngineSpeed * 6;
        
        /// <summary>
        /// Sets throttle input
        /// </summary>
        /// <param name="input">Throttle input</param>
        public void SetThrottle(float input)
        {
            _throttle = Mathf.Clamp01(input);
        }
        
        /// <summary>
        /// Applies engine (prop) force
        /// </summary>
        private void FixedUpdate()
        {
            _rigidbody.AddForceAtPosition(transform.forward * (_throttle * MaxForce), transform.position);
        }
        
        /// <summary>
        /// Drag debug gizmo
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * _throttle * MaxForce);
            
        #if UNITY_EDITOR
            Handles.Label(transform.position, $"{(int)(_throttle * 100)}%");
        #endif
        }
    }
}
