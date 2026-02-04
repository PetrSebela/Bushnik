using System.Collections.Generic;
using UnityEngine;

namespace Aircraft.Components
{
    /// <summary>
    /// Class responsible for animation of suspension components
    /// </summary>
    public class SuspensionKinematics : MonoBehaviour
    {
        /// <summary>
        /// Shock modeling the physics of animated landing gear
        /// </summary>
        [SerializeField] private Shock shock;
        
        /// <summary>
        /// Shock body
        /// </summary>
        [SerializeField] private Transform shockBody;
        
        /// <summary>
        /// Shock strut that extends to connect the lower controls arm to the wheel
        /// </summary>
        [SerializeField] private Transform shockStrut;
        
        /// <summary>
        /// Upper suspension assembly containing the wheel and arm
        /// </summary>
        [SerializeField] private Transform suspensionAssembly;
        
        
        /// <summary>
        /// Rotates suspension component along front axis to solve the suspension kinematics
        /// </summary>
        /// <param name="component"></param>
        /// <param name="target"></param>
        private void LookAtPosition(Transform component, Vector3 target)
        {
            var constrainedTarget = new Plane(transform.forward, component.position).ClosestPointOnPlane(target);
            var direction = constrainedTarget - component.position;
            component.rotation = Quaternion.LookRotation(direction, transform.forward);
        }
        
        /// <summary>
        /// Sets rotation of all suspension components
        /// </summary>
        void Update()
        {
            var wheelPosition= shock.GetWheelPosition();
            LookAtPosition(shockBody, wheelPosition);
            LookAtPosition(shockStrut, wheelPosition);
            LookAtPosition(suspensionAssembly, wheelPosition);
        }
    }
}
