using UnityEngine;

namespace Utility
{
    /// <summary>
    /// Collection of random yet helpful functions
    /// </summary>
    public class Generic : MonoBehaviour
    {
        /// <summary>
        /// Located first instance of provided type while traversing the scene hierarchy from bottom up
        /// </summary>
        /// <param name="parent">Parent from which the search should start</param>
        /// <typeparam name="T">Type to be located</typeparam>
        /// <returns>First instance of provided type</returns>
        public static T LocateObjectTowardsRoot<T>(Transform parent) where T : Component
        {
            var searched = parent.gameObject.GetComponent<T>();
            
            if (searched != null)
                return searched;
            
            if (parent.parent == null)
                return null;
            
            return LocateObjectTowardsRoot<T>(parent.parent);
        }

        /// <summary>
        /// Draws transform axis (for some reason unity does not correctly show the axis when the component has no mesh filter and renderer )
        /// </summary>
        /// <param name="transform">Transform for which the axis will be rendered</param>
        public static void DrawDirectionGizmo(Transform transform)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.right);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.up);
        }
    }
}
