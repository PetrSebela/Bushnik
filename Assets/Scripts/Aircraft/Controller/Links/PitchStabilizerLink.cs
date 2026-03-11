using Aircraft.Controller.Assistants;
using UnityEngine;

namespace Aircraft.Controller.Links
{
    /// <summary>
    /// Link used for stabilizing aircraft pitch attitude in world space
    /// </summary>
    public class PitchStabilizerLink : InputLink
    {
        /// <summary>
        /// Preceding signal chain link
        /// </summary>
        [SerializeField] private InputLink source;
        
        /// <summary>
        /// PID controller
        /// </summary>
        [SerializeField] private PitchAxisController controller;

        /// <summary>
        /// Transition time between user and assist inputs
        /// </summary>
        [SerializeField] private float transitionTime;
        
        /// <summary>
        /// If PID controller target was set
        /// </summary>
        private bool _updatedTarget = false;
        
        /// <summary>
        /// Mix value
        /// </summary>
        private float _mix = 0;
        
        /// <summary>
        /// Smooths user input and updates mixer value
        /// </summary>
        public void Update()
        {
            if (!_updatedTarget && Mathf.Abs(source.GetOutput()) <= 0.05f)
            {
                controller.SetAttitude();
                _updatedTarget = true;
            }

            if (Mathf.Abs(source.GetOutput()) > 0.05f)
            {
                _updatedTarget = false;
                _mix -= Time.deltaTime / transitionTime;
            }
            else
            {
                _mix += Time.deltaTime / transitionTime;
            }

            _mix = Mathf.Clamp01(_mix);
        }
        
        /// <summary>
        /// Used for propagating input further up the signal chain
        /// </summary>
        /// <returns>Link output</returns>
        public override float GetOutput()
        {
            var input = Mathf.Lerp(source.GetOutput(), controller.GetCommand(), _mix);
            return Mathf.Clamp(input, -1, 1);
        }
    }
}
