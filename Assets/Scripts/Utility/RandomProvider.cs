using UnityEngine;

namespace Utility
{
    /// <summary>
    /// Class for management of randomness
    /// </summary>
    public static class RandomProvider
    {

        /// <summary>
        /// Generates random points in square area
        /// </summary>
        /// <param name="center">Square center</param>
        /// <param name="side">Square side</param>
        /// <param name="count">Number of samples</param>
        /// <returns>Array of samples</returns>
        public static Vector3[] GetRandomPointsIn(Vector3 center, float side, int count)
        {
            Vector3[] samples = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                var position = center;
                position.x += Random.Range(-0.5f, 0.5f) * side;
                position.z += Random.Range(-0.5f, 0.5f) * side;
                samples[i] = position;
            }
            
            return samples;
        }
    }
}
