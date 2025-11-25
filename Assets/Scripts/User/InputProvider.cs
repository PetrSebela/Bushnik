using UnityEngine;

namespace User
{
    /// <summary>
    /// Class for managing unity input actions
    /// </summary>
    /// <remarks>
    /// Input callbacks should only be registered after initial Awake 
    /// </remarks>
    public class InputProvider : MonoBehaviour
    {
        /// <summary>
        /// Private input object reference
        /// </summary>
        private SimInput _input = null;
        
        /// <summary>
        /// Public input object getter
        /// </summary>
        public SimInput Input => _input;
        
        
        /// <summary>
        /// Private singleton reference
        /// </summary>
        private static InputProvider _instance;

        /// <summary>
        /// Singleton getter
        /// </summary>
        public static InputProvider Instance => _instance;

        /// <summary>
        /// Singleton initialization
        /// </summary>
        public void Awake()
        {
            _instance = this;
            _input = new();
        }

        /// <summary>
        /// Enable input object
        /// </summary>
        public void OnEnable()
        {
            _input.Enable();
        }

        /// <summary>
        /// Disable input object
        /// </summary>
        public void OnDisable()
        {
            _input.Disable();
        }
    }
}
