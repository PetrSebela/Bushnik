using System;
using UnityEngine;
using UnityEngine.InputSystem;
using User;

namespace Aircraft
{
    /// <summary>
    /// Class for interfacing aircraft with unity input system
    /// </summary>
    public class AircraftController : MonoBehaviour
    {
        /// <summary>
        /// Controlled aircraft
        /// </summary>
        [SerializeField] Aircraft aircraft;
        
        /// <summary>
        /// Input smoothing factor
        /// </summary>
        [SerializeField] private float inputSmoothing;

        /// <summary>
        /// Current throttle setting
        /// </summary>
        public float Throttle = 0;
        
        /// <summary>
        /// User throttle input
        /// </summary>
        private float _throttleInput = 0;
        
        /// <summary>
        /// Throttle sensitivity
        /// </summary>
        public float ThrottleSensitivity = 0.01f;
        
        /// <summary>
        /// User pitch input
        /// </summary>
        public float PitchInput = 0;
        
        /// <summary>
        /// Processed pitch input
        /// </summary>
        private float _pitchInput = 0;
        
        /// <summary>
        /// User roll input
        /// </summary>
        public float RollInput = 0;
        
        /// <summary>
        /// Processed roll input
        /// </summary>
        private float _rollInput = 0;
        
        /// <summary>
        /// User yaw input
        /// </summary>
        public float YawInput = 0;
        
        /// <summary>
        /// Processed yaw input
        /// </summary>
        private float _yawInput = 0;
        
        /// <summary>
        /// Left brake input
        /// </summary>
        public float LeftBrakeInput = 0;
        
        /// <summary>
        /// Right brake input
        /// </summary>
        public float RightBrakeInput = 0;
        
        void Start()
        {
            RegisterInput();
        }
        
        void Update()
        {
            Throttle += _throttleInput * ThrottleSensitivity;
            Throttle = Mathf.Clamp(Throttle, 0, 1);
            aircraft.SetThrottleInput(Throttle);
            
            aircraft.SetPitchInput(PitchInput);
            aircraft.SetRollInput(RollInput);

            _yawInput = Mathf.Lerp(_yawInput, YawInput, Time.deltaTime * inputSmoothing);
            aircraft.SetYawInput(_yawInput);
            
            _pitchInput = Mathf.Lerp(_pitchInput, PitchInput, Time.deltaTime * inputSmoothing);
            aircraft.SetPitchInput(_pitchInput);
            
            _rollInput = Mathf.Lerp(_rollInput, RollInput, Time.deltaTime * inputSmoothing);
            aircraft.SetRollInput(_rollInput);
            
            aircraft.SetLeftBrakeInput(LeftBrakeInput);
            aircraft.SetRightBrakeInput(RightBrakeInput);
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
            PitchInput = context.ReadValue<float>();
        }

        void OnPitchCancelled(InputAction.CallbackContext context)
        {
            PitchInput = 0;
        }

        void OnRollPerformed(InputAction.CallbackContext context)
        {
            RollInput = context.ReadValue<float>();
        }

        void OnRollCancelled(InputAction.CallbackContext context)
        {
            RollInput = 0;
        }

        void OnYawPerformed(InputAction.CallbackContext context)
        {
            YawInput = context.ReadValue<float>();
        }

        void OnYawCancelled(InputAction.CallbackContext context)
        {
            YawInput = 0;
        }

        void OnLeftBrakePerformed(InputAction.CallbackContext context)
        {
            LeftBrakeInput = context.ReadValue<float>();
        }

        void OnLeftBrakeCancelled(InputAction.CallbackContext context)
        {
            LeftBrakeInput = 0;
        }

        void OnRightBrakePerformed(InputAction.CallbackContext context)
        {
            RightBrakeInput = context.ReadValue<float>();
        }

        void OnRightBrakeCancelled(InputAction.CallbackContext context)
        {
            RightBrakeInput = 0;
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
            
            InputProvider.Instance.Input.Aircraft.Leftbrake.performed += OnLeftBrakePerformed;
            InputProvider.Instance.Input.Aircraft.Leftbrake.canceled += OnLeftBrakeCancelled;
            
            InputProvider.Instance.Input.Aircraft.Rightbrake.performed += OnRightBrakePerformed;
            InputProvider.Instance.Input.Aircraft.Rightbrake.canceled += OnRightBrakeCancelled;
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
            
            InputProvider.Instance.Input.Aircraft.Leftbrake.performed -= OnLeftBrakePerformed;
            InputProvider.Instance.Input.Aircraft.Leftbrake.canceled -= OnLeftBrakeCancelled;
            
            InputProvider.Instance.Input.Aircraft.Rightbrake.performed -= OnRightBrakePerformed;
            InputProvider.Instance.Input.Aircraft.Rightbrake.canceled -= OnRightBrakeCancelled;
        }
    }
}
