using Aircraft.Controller.Assistants;
using Aircraft.Controller.Links;
using UnityEngine;
using UnityEngine.InputSystem;
using User;

namespace Aircraft.Controller
{
    /// <summary>
    /// Class for interfacing aircraft with unity input system
    /// </summary>
    public class AircraftController : MonoBehaviour
    {
        /// <summary>
        /// Controlled aircraft
        /// </summary>
        [SerializeField] private Aircraft aircraft;
        
        /// <summary>
        /// Getter of currently controller aircraft
        /// </summary>
        public Aircraft Aircraft => aircraft;
        
        /// <summary>
        /// Pitch input channel
        /// </summary>
        [SerializeField] private InputLink pitchInput;
        
        /// <summary>
        /// Roll input channel
        /// </summary>
        [SerializeField] private InputLink rollInput;
        
        /// <summary>
        /// Yaw input channel
        /// </summary>
        [SerializeField] private InputLink yawInput;
        
        /// <summary>
        /// Throttle input channel
        /// </summary>
        [SerializeField] private InputLink throttleInput;
        
        /// <summary>
        /// Brake input channel
        /// </summary>
        [SerializeField] private InputLink brakeInput;

        /// <summary>
        /// Flap input channel
        /// </summary>
        [SerializeField] private InputLink flapInput;
        
        /// <summary>
        /// Throttle input ( needs to be exposed for UI )
        /// </summary>
        public float throttle => throttleInput.GetOutput();
        
        /// <summary>
        /// Brake input ( needs to be exposed for UI )
        /// </summary>
        public float brake => brakeInput.GetOutput();
        
        void Update()
        {
            aircraft.SetPitchInput(pitchInput.GetOutput());
            aircraft.SetRollInput(rollInput.GetOutput());
            aircraft.SetYawInput(yawInput.GetOutput());
            aircraft.SetFlapAngle(flapInput.GetOutput());
            
            aircraft.SetThrottleInput(throttle);
            aircraft.SetBrakeInput(brake, brake);
        }
    }
}
