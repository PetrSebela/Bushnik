using Aircraft.Controller.Assistants;
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
        
        public Aircraft Aircraft => aircraft;
        
        /// <summary>
        /// Current throttle setting
        /// </summary>
        public float throttle = 0;
        
        /// <summary>
        /// User throttle input
        /// </summary>
        private float _throttleInput = 0;
        
        /// <summary>
        /// Throttle sensitivity
        /// </summary>
        public float throttleSensitivity = 0.01f;
        
        /// <summary>
        /// Input mixer for pitch axis
        /// </summary>
        [SerializeField] private InputMixer pitchInput;
        
        /// <summary>
        /// Input mixer for roll axis
        /// </summary>
        [SerializeField] private InputMixer rollInput;
        
        /// <summary>
        /// Input mixer for yaw axis
        /// </summary>
        [SerializeField] private InputMixer yawInput;
        
        /// <summary>
        /// Pitch axis stabilizer
        /// </summary>
        [SerializeField] private PitchAxisController pitchController;
        
        void Start()
        {
            RegisterInput();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        void Update()
        {
            throttle += _throttleInput * throttleSensitivity;
            throttle = Mathf.Clamp(throttle, 0, 1);
            aircraft.SetThrottleInput(throttle);
            
            var pitchCorrectCommand = pitchController.GetCommand();
            pitchInput.SetAssistInput(pitchCorrectCommand);
            var pitch = pitchInput.GetMixedInput;
            aircraft.SetPitchInput(pitch);

            aircraft.SetRollInput(rollInput.GetMixedInput);
            
            aircraft.SetYawInput(yawInput.GetMixedInput);
        }
        
        void OnThrottlePerformed(InputAction.CallbackContext context)
        {
            _throttleInput = context.ReadValue<float>();
        }

        void OnThrottleCancelled(InputAction.CallbackContext context)
        {
            _throttleInput = 0;
        }

        void OnPitchPerformed(InputAction.CallbackContext context)
        {
            var input = context.ReadValue<float>();
            pitchInput.SetPlayerInput(input);
        }

        void OnPitchCancelled(InputAction.CallbackContext context)
        {
            pitchInput.SetPlayerInput(0f);
        }

        void OnRollPerformed(InputAction.CallbackContext context)
        {
            var input = context.ReadValue<float>();
            rollInput.SetPlayerInput(input);
        }

        void OnRollCancelled(InputAction.CallbackContext context)
        {
            rollInput.SetPlayerInput(0);
        }

        void OnYawPerformed(InputAction.CallbackContext context)
        {
            var input =  context.ReadValue<float>();
            yawInput.SetPlayerInput(input);
        }

        void OnYawCancelled(InputAction.CallbackContext context)
        {
            yawInput.SetPlayerInput(0);
        }
        
        private void RegisterInput()
        {
            InputProvider.Instance.Input.Aircraft.Throttle.performed += OnThrottlePerformed;
            InputProvider.Instance.Input.Aircraft.Throttle.canceled += OnThrottleCancelled;
            
            InputProvider.Instance.Input.Aircraft.Pitch.performed += OnPitchPerformed;
            InputProvider.Instance.Input.Aircraft.Pitch.canceled += OnPitchCancelled;
            
            InputProvider.Instance.Input.Aircraft.Roll.performed += OnRollPerformed;
            InputProvider.Instance.Input.Aircraft.Roll.canceled += OnRollCancelled;
         
            InputProvider.Instance.Input.Aircraft.Yaw.performed += OnYawPerformed;
            InputProvider.Instance.Input.Aircraft.Yaw.canceled += OnYawCancelled;
        }

        private void OnDisable()
        {
            InputProvider.Instance.Input.Aircraft.Throttle.performed -= OnThrottlePerformed;
            InputProvider.Instance.Input.Aircraft.Throttle.canceled -= OnThrottleCancelled;
            
            InputProvider.Instance.Input.Aircraft.Pitch.performed -= OnPitchPerformed;
            InputProvider.Instance.Input.Aircraft.Pitch.canceled -= OnPitchCancelled;
            
            InputProvider.Instance.Input.Aircraft.Roll.performed -= OnRollPerformed;
            InputProvider.Instance.Input.Aircraft.Roll.canceled -= OnRollCancelled;
         
            InputProvider.Instance.Input.Aircraft.Yaw.performed -= OnYawPerformed;
            InputProvider.Instance.Input.Aircraft.Yaw.canceled -= OnYawCancelled;
        }
    }
}
