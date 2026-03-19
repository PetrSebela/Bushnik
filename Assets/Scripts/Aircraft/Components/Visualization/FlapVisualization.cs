using System;
using UnityEngine;

namespace Aircraft.Components.Visualization
{
    /// <summary>
    /// Component responsible for visualization of flaps
    /// </summary>
    [RequireComponent(typeof(Flap))]
    public class FlapVisualization : MonoBehaviour
    {
        /// <summary>
        /// Flap rotation axis
        /// </summary>
        [SerializeField] private Vector3 rotationAxis = Vector3.right;
        
        /// <summary>
        /// Flap model reference
        /// </summary>
        [SerializeField] private Transform flapModel;
        
        /// <summary>
        /// Visualized flap
        /// </summary>
        private Flap _flap;
        
        /// <summary>
        /// Physical flap offset
        /// </summary>
        private Quaternion _offset;
        
        private void Awake()
        {
            _flap = GetComponent<Flap>();
            _offset = flapModel.localRotation;
        }

        private void Update()
        {
            flapModel.localRotation = _offset * Quaternion.AngleAxis(_flap.Angle, rotationAxis);
        }
    }
}
