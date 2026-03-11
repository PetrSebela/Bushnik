using UnityEngine;
using UnityEngine.InputSystem;

namespace Aircraft.Controller
{
    /// <summary>
    /// Processes simple axis input
    /// </summary>
    public class AxisSource : InputSource
    {
        private void OnEnable()
        {
            inputAction.action.performed += OnInputPerformed;
            inputAction.action.canceled += OnInputCancelled;
        }

        private void OnInputPerformed(InputAction.CallbackContext context)
        {
            value = context.ReadValue<float>();
            link.SetSource(this);
        }

        private void OnInputCancelled(InputAction.CallbackContext context)
        {
            value = 0;   
        }
    }
}
