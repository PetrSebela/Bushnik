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
            // return 0.1255f;
            var height = point.y / 1000;
            return 1.255f * (20 - height) / (20 + height);
        }
    }
}
