using UnityEngine;
using UnityEngine.InputSystem;

namespace Aircraft.Controller
{
    /// <summary>
    /// Class for processing keyboard brake input 
    /// </summary>
    public class KeyboardBrake : InputSource
    {
        /// <summary>
        /// Keyboard throttle reference
        /// </summary>
        [SerializeField] private KeyboardThrottle throttle;
        
        /// <summary>
        /// Should brake
        /// </summary>
        private bool _braking = false;
        
        /// <summary>
        /// Update output based on current throttle value
        /// </summary>
        void Update()
        {
            if (throttle.value == 0 && _braking)
                value = 1;
            else
                value = 0;
        }
        
        private void OnEnable()
        {
            inputAction.action.performed += OnInputPerformed;
            inputAction.action.canceled += OnInputCancelled;
        }
    
        private void OnInputPerformed(InputAction.CallbackContext context)
        {
            _braking = true;
            link.SetSource(this);
        }

        private void OnInputCancelled(InputAction.CallbackContext context)
        {
            _braking = false;
        }
    }
}
