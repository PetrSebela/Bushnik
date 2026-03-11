using UnityEngine;

namespace Aircraft.Controller
{
    /// <summary>
    /// Link for recieving data from multiple InputSources
    /// </summary>
    public class VariableInputLink : InputLink
    {
        /// <summary>
        /// User input source
        /// </summary>
        private InputSource _source;
        
        /// <summary>
        /// Assign temporary source before user touches the controls
        /// </summary>
        private void Awake()
        {
            _source = GetComponent<InputSource>();
        }
        
        /// <summary>
        /// Sets active input source
        /// </summary>
        /// <param name="source">Input source to be used</param>
        public void SetSource(InputSource source)
        {
            _source = source;
        }
    
        /// <summary>
        /// Used for propagating input further up the signal chain
        /// </summary>
        /// <returns>Link output</returns>
        public override float GetOutput()
        {
            return Mathf.Clamp(_source.value, -1, 1);
        }
    }
}
