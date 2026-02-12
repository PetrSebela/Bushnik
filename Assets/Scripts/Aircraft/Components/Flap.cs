using System;
using Unity.Mathematics;
using UnityEngine;

namespace Aircraft.Components
{
    /// <summary>
    /// Class representing the flap
    /// This class does not produce any forces of its own and instead relies on the wing model implemented in the wing class
    /// The flap axis of rotation is assumed to be the right transform vector
    /// </summary>
    [RequireComponent(typeof(Wing))]
    public class Flap : MonoBehaviour
    {
        /// <summary>
        /// Maximum flap deflection angle
        /// </summary>
        [SerializeField] private float maxDeflection;
        
        /// <summary>
        /// Visual representation of the flap
        /// </summary>
        [SerializeField] private Transform visual;

        /// <summary>
        /// How fast the flap reacts to input
        /// </summary>
        [SerializeField] private float responsiveness;
        
        /// <summary>
        /// Original element rotation
        /// </summary>
        private Quaternion _rotationOffset;
        
        private Quaternion _visualRotationOffset;
        
        /// <summary>
        /// Caches the original wing rotation
        /// </summary>
        public void Awake()
        {
            _rotationOffset = transform.localRotation;
            _visualRotationOffset = visual.localRotation;
        }
        
        /// <summary>
        /// Rotates the wing element
        /// </summary>
        /// <param name="input">Surface deflection (-1, 1)</param>
        public void SetInput(float input)
        {
            var rotation = Quaternion.AngleAxis(input * maxDeflection, Vector3.right);
            transform.localRotation = _rotationOffset * rotation;
            visual.localRotation = _visualRotationOffset * rotation;
        }
    }
}
