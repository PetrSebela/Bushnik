using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace Buildings
{
    public class WindTurbine : MonoBehaviour
    {
        /// <summary>
        /// Revolutions per minute
        /// </summary>
        [SerializeField] private float rotationSpeed;
        
        /// <summary>
        /// Rotor transform
        /// </summary>
        [SerializeField] private Transform rotor;

        private float _angularSpeed;
        
        void Start()
        {
            rotor.localRotation *= Quaternion.AngleAxis(Random.Range(0, 360), Vector3.right);
            _angularSpeed = rotationSpeed * 6 + Random.Range(-5f,5f);
        }
        
        void Update()
        {
            rotor.localRotation *= Quaternion.AngleAxis(_angularSpeed * Time.deltaTime, Vector3.right);
        }
    }
}
