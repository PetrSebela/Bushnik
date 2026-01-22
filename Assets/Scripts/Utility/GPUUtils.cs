using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Utility
{
    /// <summary>
    /// Collection of helpful functions when interfacing with GPU
    /// </summary>
    public static class GPUUtils 
    {
        /// <summary>
        /// Creates task that asynchronously waits for buffer readback 
        /// </summary>
        /// <param name="buffer">Buffer to be read</param>
        /// <typeparam name="T">Type of elements of buffer</typeparam>
        /// <returns>Data from the buffer</returns>
        public static async Task<T[]> ReadBufferAsync<T>(ComputeBuffer buffer) where T : struct
        {
            var request = AsyncGPUReadback.Request(buffer);

            while (!request.done)
                await Task.Yield();
            
            return request.GetData<T>().ToArray();
        }
    }
}
