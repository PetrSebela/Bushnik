using System.Collections.Generic;
using TMPro;
using UnityEngine;
using User;
using Utility;

namespace UI.Interactable
{
    /// <summary>
    /// Class responsible for interacting with objects that are in its proximity
    /// </summary>
    public class InteractableInvoker : Singleton<InteractableInvoker>
    {
        /// <summary>
        /// Interactable object in range
        /// </summary>
        private Interactable _inRange;
        
        /// <summary>
        /// Interaction message text
        /// </summary>
        [SerializeField] private TMP_Text interactionMessage;
        
        /// <summary>
        /// Interaction message canvas
        /// </summary>
        [SerializeField] private CanvasGroup interactionCanvas;
        
        private void Start()
        {
            InputProvider.Instance.Input.UI.Action.performed += _ => Interact();
            interactionCanvas.alpha = 0;
        }

        private void Interact()
        {
            if(_inRange == null)
                return;
            _inRange.Interact();
        }
        
        public void InRange(Interactable interactable)
        {
            if(_inRange == null)
                LeanTween.alphaCanvas(interactionCanvas, 1, 0.125f).setIgnoreTimeScale(true);
            
            _inRange = interactable;
            interactionMessage.text = interactable.message;
        }

        public void ClearRange(Interactable interactable)
        {
            if (interactable != _inRange)
                return;
            
            _inRange = null;
            LeanTween.alphaCanvas(interactionCanvas, 0, 0.125f).setIgnoreTimeScale(true);
        }
    }
}
