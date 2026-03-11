using Aircraft.Controller.Assistants;
using UnityEngine;

namespace Aircraft.Controller
{
    public class SmoothingLink : InputLink
    {
        /// <summary>
        /// Preceding signal chain link
        /// </summary>
        [SerializeField] private InputLink source;
        
        /// <summary>
        /// Amount of smoothing applied to user input
        /// </summary>
        [SerializeField] private float smoothing;
        
        /// <summary>
        /// Smoothed user input
        /// </summary>
        private float _userInputSmooth;
        
        /// <summary>
        /// Smooths user input
        /// </summary>
        public void Update()
        {
            _userInputSmooth = Mathf.Lerp(_userInputSmooth, source.GetOutput(),smoothing * Time.deltaTime);
        }
        
        /// <summary>
        /// Used for propagating input further up the signal chain
        /// </summary>
        /// <returns>Link output</returns>
        public override float GetOutput()
        {
            return Mathf.Clamp(_userInputSmooth, -1, 1);
        }
    }
}
