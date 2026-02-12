using UnityEngine;
using Utility;

namespace Aircraft.Controller.Assistants
{
    /// <summary>
    /// Class responsible for stabilizing pitch ( vertical ) aircraft axis
    /// </summary>
    public class PitchAxisController : PIDController
    {
        /// <summary>
        /// Cutoff bank angle
        /// </summary>
        [SerializeField] private float cutoffThreshold = 65f;
        
        /// <summary>
        /// Cutoff transition width 
        /// </summary>
        [SerializeField] private float cutoffBand = 10f;
        
        /// <summary>
        /// output value
        /// </summary>
        private float _output;
        
        /// <summary>
        /// Reference to controlled aircraft
        /// </summary>
        private Aircraft _aircraft;
        
        void Start()
        {
            _aircraft = Utility.Generic.LocateObjectTowardsRoot<Aircraft>(transform.parent);
        }
        
        /// <summary>
        /// Updates PID loop
        /// </summary>
        void Update()
        {
            var reference = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            var rotated = _aircraft.AngularVelocity * reference;
            
            var referenceAngle = Vector3.Angle(reference, Vector3.up);
            var rotatedAngle = Vector3.Angle(rotated, Vector3.up);
            var delta = rotatedAngle - referenceAngle;
            
            UpdateController(delta);
        }

        /// <summary>
        /// Computes assist influence value
        /// </summary>
        /// <returns>How much should control be used</returns>
        private float GetModifier()
        {
            var angle = Vector3.Angle(transform.up, Vector3.up);
            var progress = (angle - (cutoffThreshold - cutoffBand)) / cutoffBand;
            progress = Mathf.Clamp01(progress);
            return 1 - progress;
        }
        
        /// <summary>
        /// If controller is disengaged
        /// </summary>
        public bool Disengaged => GetModifier() == 0 || !gameObject.activeInHierarchy;
        
        /// <summary>
        /// Stability assist control input
        /// </summary>
        /// <returns>Control input value</returns>
        public float GetCommand()
        {
            if(!gameObject.activeInHierarchy)
                return 0;
            return _controllerOutput * GetModifier();
        }
    }
}