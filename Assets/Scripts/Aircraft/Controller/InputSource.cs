using Aircraft.Controller.Links;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Aircraft.Controller
{
    /// <summary>
    /// Base class for processing user inputs from unity input system
    /// </summary>
    public class InputSource : MonoBehaviour
    {
        /// <summary>
        /// What action should be processed
        /// </summary>
        [SerializeField] protected InputActionReference inputAction;
        
        /// <summary>
        /// To what link should the input be outputted
        /// </summary>
        [SerializeField] protected VariableInputLink link;
        
        /// <summary>
        /// Input value
        /// </summary>
        public float value { get; protected set; } = 0;
    }
}
