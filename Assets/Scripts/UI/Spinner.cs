using UnityEngine;

namespace UI
{
    /// <summary>
    /// Spins UI element along the Z axis
    /// </summary>
    public class Spinner : MonoBehaviour
    {
        /// <summary>
        /// Rotation animation speed
        /// </summary>
        [SerializeField] private AnimationCurve velocity;
        
        void Update()
        {
            transform.localRotation *= Quaternion.AngleAxis(velocity.Evaluate(Time.realtimeSinceStartup) * Time.deltaTime, Vector3.forward);
        }
    }
}
