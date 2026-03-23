using UnityEngine;

namespace Aircraft.Components.Visualization
{
    public class SteerableWheelVisualization : MonoBehaviour
    {
        /// <summary>
        /// Wheel steer axis
        /// </summary>
        [SerializeField] private Vector3 rotationAxis = Vector3.up;
        
        /// <summary>
        /// Wheel model reference
        /// </summary>
        [SerializeField] private Transform flapModel;
        
        /// <summary>
        /// Visualized wheel
        /// </summary>
        private SteerableWheel _wheel;
        
        /// <summary>
        /// Physical wheel offset
        /// </summary>
        private Quaternion _offset;
        
        private void Awake()
        {
            _wheel = GetComponent<SteerableWheel>();
            _offset = flapModel.localRotation;
        }

        private void Update()
        {
            flapModel.localRotation = _offset * Quaternion.AngleAxis(_wheel.CurrentAngle, rotationAxis);
        }
    }
}
