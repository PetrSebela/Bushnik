using UnityEngine;

namespace Aircraft.Controller
{
    /// <summary>
    /// Base class forming input signal chain
    /// </summary>
    public abstract class InputLink : MonoBehaviour
    {
        /// <summary>
        /// Used for propagating input further up the signal chain
        /// </summary>
        /// <returns>Link output</returns>
        public abstract float GetOutput();
    }
}
