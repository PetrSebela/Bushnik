using UnityEngine;

namespace Utility
{
    /// <summary>
    /// Class for management of randomness
    /// </summary>
    public static class RandomProvider
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
                var position = min;

                position.x += Random.Range(-0.5f, 0.5f) * delta.x;
                
                if(delta.y != 0)
                    position.y += Random.Range(-0.5f, 0.5f) * delta.y;
                
                position.z += Random.Range(-0.5f, 0.5f) * delta.z;

                samples[i] = position;
            }
            
            return samples;
        }
    }
}
