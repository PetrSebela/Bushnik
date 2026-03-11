using Unity.VisualScripting;
using UnityEngine;

namespace Aircraft.Controller.Links
{
    /// <summary>
    /// Manages brake input so that the plane does not tip over while braking
    /// </summary>
    public class BrakeRegulatorLink : InputLink
    {
        /// <summary>
        /// Preceding link
        /// </summary>
        [SerializeField] private InputLink source;
        
        /// <summary>
        /// Maximum allowed tip angle during braking
        /// </summary>
        [SerializeField] private float maxTipAngle = 0;
        
        /// <summary>
        /// Range over which the brake input will be modulated
        /// </summary>
        [SerializeField] private float brakeFadeRange = 2;
        
        /// <summary>
        /// Aircraft reference
        /// </summary>
        private Aircraft _aircraft;

        private void Awake()
        {
            _aircraft = Utility.Generic.LocateObjectTowardsRoot<Aircraft>(transform.parent);
        }
        
        public override float GetOutput()
        {
            var raw = source.GetOutput();
            var angle = 90 -Vector3.Angle(_aircraft.transform.forward, Vector3.up);
            
            if (angle < maxTipAngle)
                return 0f;

            var modifier = Mathf.Clamp01((angle - maxTipAngle) / brakeFadeRange);
            return Mathf.Clamp(raw * modifier, 0.0f, 1.0f);
        }
    }
}
