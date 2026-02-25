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
        
        /// <summary>
        /// Getter of currently controller aircraft
        /// </summary>
        public Aircraft Aircraft => aircraft;
        
        /// <summary>
        /// Current throttle setting
        /// </summary>
        public float throttle = 0;

        /// <summary>
        /// Current brake input
        /// </summary>
        public float brake = 0;
        
        /// <summary>
        /// User throttle input from gamepad
        /// </summary>
        private float _throttleInput = 0;

        /// <summary>
        /// User brake input from gamepad
        /// </summary>
        private float _brakeInput = 0;

        /// <summary>
        /// User throttle / brake input from keyboard
        /// </summary>
        private float _throttleBrakeComboInput = 0;
        
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

        /// <summary>
        /// Use used keyboard for input
        /// </summary>
        private bool _keyboardInput = true;
        
        void Start()
        {
            RegisterInput();
        }

        void ProcessKeyboardThrottle()
        {
            throttle += _throttleBrakeComboInput * throttleSensitivity;
            throttle = Mathf.Clamp(throttle, 0, 1);

            if (throttle == 0 && _throttleBrakeComboInput < 0)
                brake = Mathf.Abs(_throttleBrakeComboInput);
            else
                brake = 0;
            
            aircraft.SetBrakeInput(brake, brake);
            aircraft.SetThrottleInput(throttle);
        }

        void ProcessGamepadThrottle()
        {
            throttle = _throttleInput;
            brake = _brakeInput;
            
            aircraft.SetBrakeInput(brake, brake);
            aircraft.SetThrottleInput(throttle);
        }
        
        void Update()
        {
            if(_keyboardInput)
                ProcessKeyboardThrottle();
            else
                ProcessGamepadThrottle();
            
            var pitchCorrectCommand = pitchController.GetCommand();
            pitchInput.SetAssistInput(pitchCorrectCommand);
            var pitch = pitchInput.GetMixedInput;
            aircraft.SetPitchInput(pitch);
            
            aircraft.SetRollInput(rollInput.GetMixedInput);
            aircraft.SetYawInput(yawInput.GetMixedInput);
        }
        
        void OnThrottlePerformed(InputAction.CallbackContext context)
        {
            _keyboardInput = false;
            _throttleInput = context.ReadValue<float>();
        }

        void OnThrottleCancelled(InputAction.CallbackContext context)
        {
            _keyboardInput = false;
            _throttleInput = 0;
        }

        void OnBrakePerformed(InputAction.CallbackContext context)
        {
            _keyboardInput = false;
            _brakeInput = context.ReadValue<float>();
        }

        void OnBrakeCancelled(InputAction.CallbackContext context)
        {
            _keyboardInput = false;
            _brakeInput = 0;
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

        void OnThrottleBrakeComboPerformed(InputAction.CallbackContext context)
        {
            _keyboardInput = true;
            _throttleBrakeComboInput = context.ReadValue<float>();
        }

        void OnThrottleBrakeComboCancelled(InputAction.CallbackContext context)
        {
            _keyboardInput = true;
            _throttleBrakeComboInput = 0;
        }
        
        private void RegisterInput()
        {
            InputProvider.Instance.Input.Aircraft.Throttle.performed += OnThrottlePerformed;
            InputProvider.Instance.Input.Aircraft.Throttle.canceled += OnThrottleCancelled;
            
            InputProvider.Instance.Input.Aircraft.Brake.performed += OnBrakePerformed;
            InputProvider.Instance.Input.Aircraft.Brake.canceled += OnBrakeCancelled;
            
            InputProvider.Instance.Input.Aircraft.ThrottleBrakeCombo.performed += OnThrottleBrakeComboPerformed;
            InputProvider.Instance.Input.Aircraft.ThrottleBrakeCombo.canceled += OnThrottleBrakeComboCancelled;
            
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
            
            InputProvider.Instance.Input.Aircraft.Brake.performed -= OnBrakePerformed;
            InputProvider.Instance.Input.Aircraft.Brake.canceled -= OnBrakeCancelled;
            
            InputProvider.Instance.Input.Aircraft.ThrottleBrakeCombo.performed -= OnThrottleBrakeComboPerformed;
            InputProvider.Instance.Input.Aircraft.ThrottleBrakeCombo.canceled -= OnThrottleBrakeComboCancelled;
            
            InputProvider.Instance.Input.Aircraft.Pitch.performed -= OnPitchPerformed;
            InputProvider.Instance.Input.Aircraft.Pitch.canceled -= OnPitchCancelled;
            
            InputProvider.Instance.Input.Aircraft.Roll.performed -= OnRollPerformed;
            InputProvider.Instance.Input.Aircraft.Roll.canceled -= OnRollCancelled;
         
            InputProvider.Instance.Input.Aircraft.Yaw.performed -= OnYawPerformed;
            InputProvider.Instance.Input.Aircraft.Yaw.canceled -= OnYawCancelled;
        }
    }
}
