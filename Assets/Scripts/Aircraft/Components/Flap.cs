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
        /// Current flap angle
        /// </summary>
        private float _angle = 0;
        
        /// <summary>
        /// Current flap angle
        /// </summary>
        public float Angle => _angle;
        
        /// <summary>
        /// Original element rotation
        /// </summary>
        private Quaternion _rotationOffset;
        
        /// <summary>
        /// Caches the original wing rotation
        /// </summary>
        public void Awake()
        {
            _rotationOffset = transform.localRotation;
        }
        
        /// <summary>
        /// Rotates the wing element
        /// </summary>
        /// <param name="input">Surface deflection (-1, 1)</param>
        public void SetInput(float input)
        {
            _angle = Mathf.Clamp(input * maxDeflection,-maxDeflection, maxDeflection);
            var rotation = Quaternion.AngleAxis(_angle, Vector3.right);
            transform.localRotation = _rotationOffset * rotation;
        }

        /// <summary>
        /// Rotates the wing element to specified angle
        /// </summary>
        /// <param name="angle"></param>
        public void SetAngle(float angle)
        {
            _angle = Mathf.Clamp(angle,-maxDeflection, maxDeflection);
            var rotation = Quaternion.AngleAxis(_angle, Vector3.right);
            transform.localRotation = _rotationOffset * rotation;
        }
    }
}
