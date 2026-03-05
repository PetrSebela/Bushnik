using UnityEngine;
using UnityEngine.Events;

namespace UI.Interactable
{
    /// <summary>
    /// Base class for interactable objects
    /// </summary>
    public class Interactable : MonoBehaviour
    {
        /// <summary>
        /// Context message
        /// </summary>
        public string message;

        /// <summary>
        /// Is interaction enabled
        /// </summary>
        private bool _enabled = true;
        
        /// <summary>
        /// Is interaction enabled
        /// </summary>
        public bool Enabled => _enabled;
        
        /// <summary>
        /// Enable interaction
        /// </summary>
        public void Enable()
        {
            _enabled = true;
        }

        /// <summary>
        /// Disable interaction
        /// </summary>
        public void Disable()
        {
            _enabled = false;
            InteractableInvoker.Instance.ClearRange(this);
        }
        
        /// <summary>
        /// Registers interaction when in trigger area
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerStay(Collider other)
        {
            if (!_enabled)
                return;
            InteractableInvoker.Instance.InRange(this);
        }
        
        /// <summary>
        /// Clears interaction when leaving the trigger zone
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other)
        {
            InteractableInvoker.Instance.ClearRange(this);
        }

        /// <summary>
        /// Invokes interaction
        /// </summary>
        public virtual void Interact()
        {
            
        }
    }
}
