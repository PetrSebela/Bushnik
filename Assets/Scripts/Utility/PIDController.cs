using UnityEngine;

namespace Utility
{
    /// <summary>
    /// Basic implementation of PID controller
    /// </summary>
    public class PIDController : MonoBehaviour
    {
        /// <summary>
        /// How much the output is influenced by current error (P)
        /// </summary>
        [SerializeField] private float errorGain = 1f;
        
        /// <summary>
        /// How much the accumulated error influences the output (i)
        /// </summary>
        [SerializeField] private float offsetGain = 1f;
        
        /// <summary>
        /// Damping of the output (D)
        /// </summary>
        [SerializeField] private float damping = 1f;
        
        /// <summary>
        /// Output range of the controller
        /// </summary>
        [SerializeField] private Vector2 outputRange = Vector2.up;

        /// <summary>
        /// Maximum accumulated error
        /// </summary>
        [SerializeField] private float accumulatorSize = 1;
        
        /// <summary>
        /// Controller output
        /// </summary>
        protected float _controllerOutput;
        
        /// <summary>
        /// Error accumulator
        /// </summary>
        private float _accumulator;

        /// <summary>
        /// Last error value
        /// </summary>
        private float _lastError;

        /// <summary>
        /// Target value
        /// </summary>
        private float _target;
        
        /// <summary>
        /// Updates the PID controller value (should be called only once per frame)
        /// </summary>
        protected void UpdateController(float current)
        {
            var error = _target - current;
            
            _accumulator = Mathf.Clamp(_accumulator + error * errorGain, -accumulatorSize, accumulatorSize);
            
            var derivative = (error * errorGain - _lastError) / Time.fixedDeltaTime;
            _lastError = error * errorGain;
            
            var output = error * errorGain + _accumulator * offsetGain - derivative * damping;
            _controllerOutput = Mathf.Clamp(output, outputRange.x, outputRange.y);
        }

        /// <summary>
        /// Sets target controller value
        /// </summary>
        /// <param name="target">target value</param>
        public void SetTarget(float target)
        {
            _target = target;
        }
    }
}
