using UnityEngine;
using UnityEngine.InputSystem;

namespace Aircraft.Controller
{
    /// <summary>
    /// Keyboard throttle logic
    /// </summary>
    public class KeyboardThrottle : InputSource
    {
        /// <summary>
        /// Keyboard sensitivity
        /// </summary>
        [SerializeField] private float sensitivity = 0.0025f;
        
        /// <summary>
        /// Current throttle input
        /// </summary>
        private float _input = 0;
        
        void Update()
        {
            value += _input * sensitivity * Time.deltaTime;
            value = Mathf.Clamp(value, 0, 1);
        }
        
        private void OnEnable()
        {
            inputAction.action.performed += OnInputPerformed;
            inputAction.action.canceled += OnInputCancelled;
        }

        private void OnInputPerformed(InputAction.CallbackContext context)
        {
            _input = context.ReadValue<float>();
            link.SetSource(this);
        }

        private void OnInputCancelled(InputAction.CallbackContext context)
        {
            _input = 0;   
        }
    }
}