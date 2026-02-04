using UnityEngine;

namespace Utility
{
    public class FollowObject : MonoBehaviour
    {
        [SerializeField] Transform _target;
        [SerializeField] float _smoothing;
        [SerializeField] private Vector3 _desiredOffset = new (-5, -2, 0);
        [SerializeField] private float maxDistance = 30f;

        void Update()
        {
            var desiredPosition = _target.TransformPoint(_desiredOffset);;
            var delta = Vector3.Distance(desiredPosition, transform.position);
            transform.position = Vector3.Slerp(transform.position, desiredPosition, Time.deltaTime * _smoothing * (1 + delta / maxDistance)); 
            transform.LookAt(_target, Vector3.up);
        }
    }
}
