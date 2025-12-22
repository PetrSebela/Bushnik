using UnityEngine;

namespace Utility
{
    /// <summary>
    /// Class for management of randomness
    /// </summary>
    public class RandomProvider
    {
        /// <summary>
        /// Returns array of random points in cube specified by provided parameters
        /// </summary>
        /// <param name="min">Min corner of cube</param>
        /// <param name="max">Max corner of cube</param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Vector3[] GetRandomPointsIn(Vector3 min, Vector3 max, int count)
        {
            Vector3[] samples = new Vector3[count];
            Vector3 delta = max - min;
            
            for (int i = 0; i < count; i++)
            {
                samples[i] = new Vector3(
                    Random.Range(-0.5f, 0.5f) * delta.x,
                    Random.Range(-0.5f, 0.5f) * delta.y,
                    Random.Range(-0.5f, 0.5f) * delta.z
                    ) + min;
            }
            
            return samples;
        }
    }
}
