using UnityEngine;

namespace Aircraft.Controller
{
    /// <summary>
    /// Class responsible for processing and mixing user and assist inputs
    /// </summary>
    public class InputMixer : MonoBehaviour
    {
        /// <summary>
        /// Transition time between user and assist inputs
        /// </summary>
        [SerializeField] private float transitionTime;
        
        /// <summary>
        /// Amount of smoothing applied to user input
        /// </summary>
        [SerializeField] private float userInputSmoothing;
        
        /// <summary>
        /// Raw user input
        /// </summary>
        private float _userInputRaw;
        
        /// <summary>
        /// Smoothed user input
        /// </summary>
        private float _userInputSmooth;
        
        /// <summary>
        /// Assist input for given axis
        /// </summary>
        private float _assistInput;
        
        /// <summary>
        /// Value representing transition between user and assist inputs
        /// </summary>
        private float _mixer;
        
        /// <summary>
        /// Smooths user input and updates mixer value
        /// </summary>
        public void Update()
        {
            _userInputSmooth = Mathf.Lerp(_userInputSmooth, _userInputRaw, userInputSmoothing * Time.deltaTime);
            
            _mixer += (_userInputRaw != 0 ? 1 : -1) * Time.deltaTime / transitionTime;
            _mixer = Mathf.Clamp01(_mixer);
        }

        /// <summary>
        /// Mixed user and assistant inputs
        /// </summary>
        // public float GetMixedInput => Mathf.Lerp(_assistInput, _userInputSmooth, _mixer);
        public float GetMixedInput => Mathf.Clamp(_assistInput +_userInputSmooth, -1,1);
        
        /// <summary>
        /// Sets user input
        /// </summary>
        /// <param name="input">input value for given axis</param>
        public void SetPlayerInput(float input)
        {
            _userInputRaw = input;
        }

        /// <summary>
        /// Sets assist input
        /// </summary>
        /// <param name="input">input value for given axis</param>
        public void SetAssistInput(float input)
        {
            _assistInput = input;
        }
    }
}
