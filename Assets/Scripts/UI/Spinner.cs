using UnityEngine;

namespace UI
{
    /// <summary>
    /// Spins UI element along the Z axis
    /// </summary>
    public class Spinner : MonoBehaviour
    {
        /// <summary>
        /// Rotation speed
        /// </summary>
        [SerializeField] private float velocity;
        
        void Update()
        {
            transform.localRotation *= Quaternion.AngleAxis(velocity * Time.deltaTime, Vector3.forward);
        }
    }
}
