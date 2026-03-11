using UnityEngine;

namespace Aircraft.Controller.Links
{
    public class SpeedAttenuatorLink : InputLink
    {
        /// <summary>
        /// Preceding link
        /// </summary>
        [SerializeField] private InputLink source;
        
        /// <summary>
        /// Attenuation based on airspeed
        /// </summary>
        [SerializeField] private AnimationCurve attenuation;
        
        private Aircraft _aircraft;
        
        private void Awake()
        {
            _aircraft = Utility.Generic.LocateObjectTowardsRoot<Aircraft>(transform.parent);    
        }

        public override float GetOutput()
        {
            var input = source.GetOutput();
            
            if(!gameObject.activeSelf)
                return input;
            
            return input * attenuation.Evaluate(_aircraft.Velocity);
        }
    }
}
