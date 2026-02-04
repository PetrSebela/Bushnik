using UnityEngine;

namespace Aircraft
{
    /// <summary>
    /// Basic atmosphere model
    /// </summary>
    public class Atmosphere : MonoBehaviour
    {
        /// <summary>
        /// Computes air density at said point
        /// </summary>
        /// <param name="point">Sample point</param>
        /// <returns>Air density</returns>
        public static float GetDensityAtPoint(Vector3 point)
        {
            // TODO: Add propper model 
            return 0.1255f;
        }
    }
}
