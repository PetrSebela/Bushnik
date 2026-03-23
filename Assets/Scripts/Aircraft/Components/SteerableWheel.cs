using UnityEngine;

namespace Aircraft.Components
{
    /// <summary>
    /// Tail wheel component
    /// </summary>
    public class SteerableWheel : Shock
    {
        /// <summary>
        /// Maximum steer angle
        /// </summary>
        [SerializeField] private float maxAngle;
        
        /// <summary>
        /// Tail wheel visualization
        /// </summary>
        // [SerializeField] private Transform visualization;
        
        private float _currentAngle;
        
        public float CurrentAngle => _currentAngle;
        
        /// <summary>
        /// Sets tail wheel steer input
        /// </summary>
        /// <param name="input">Steer input</param>
        public void SetSteerInput(float input)
        {
            transform.localRotation = Quaternion.Euler(0f, input * maxAngle, 0f);  
            _currentAngle = input * maxAngle;
            // visualization.localRotation = Quaternion.Euler(0f, 0f, input * maxAngle);
        }
    }
}
