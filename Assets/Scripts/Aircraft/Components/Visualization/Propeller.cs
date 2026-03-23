using UnityEngine;

namespace Aircraft.Components.Visualization
{
    [RequireComponent(typeof(Engine))]
    public class Propeller : MonoBehaviour
    {
        /// <summary>
        /// Propeller rotation axis
        /// </summary>
        [SerializeField] private Vector3 rotationAxis = Vector3.right;
        
        /// <summary>
        /// Flap model reference
        /// </summary>
        [SerializeField] private Transform propellerTransform;
        
        /// <summary>
        /// Visualized flap
        /// </summary>
        private Engine _engine;
        
        private void Awake()
        {
            _engine = GetComponent<Engine>();
        }

        private void Update()
        {
            propellerTransform.localRotation *= Quaternion.AngleAxis(_engine.CrankshaftAngularVelocity * Time.deltaTime, rotationAxis);
        }
    }
}
