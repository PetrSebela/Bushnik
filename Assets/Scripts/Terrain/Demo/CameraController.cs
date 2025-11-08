using UnityEngine;

namespace Terrain.Demo
{
    /// <summary>
    /// Extremely basic camera controller that allows unrestricted movement around scene
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float cameraSpeed = 10;
        [SerializeField] private float mouseSensitivity = 20;

        private Vector2 _cameraAttitude = Vector2.zero;

        private Vector3 _direction = Vector3.zero;
        private Quaternion _cameraOrientation = Quaternion.identity;
        
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            // Camera rotation
            var delta = Input.mousePositionDelta * mouseSensitivity;
            _cameraAttitude += new Vector2(delta.x, delta.y);
            _cameraAttitude = new Vector2(_cameraAttitude.x, Mathf.Clamp(_cameraAttitude.y, -90, 90));
            
            // Rotate camera
            var horizontalRotation = Quaternion.AngleAxis(_cameraAttitude.x, Vector3.up);
            var verticalRotation = Quaternion.AngleAxis(-_cameraAttitude.y, Vector3.right);
            
            _cameraOrientation = horizontalRotation * verticalRotation;
            
            var forward = horizontalRotation * Vector3.forward * Input.GetAxis("Vertical");
            var right =  horizontalRotation * Vector3.right * Input.GetAxis("Horizontal");

            float verticalInput = (Input.GetKey(KeyCode.Space) ? 1 : 0) + (Input.GetKey(KeyCode.LeftControl) ? -1 : 0);
            var up = Vector3.up * verticalInput;

            _direction = forward + right + up;
        }

        void FixedUpdate()
        {
            transform.rotation = _cameraOrientation;
            transform.position += Time.fixedDeltaTime * cameraSpeed * _direction;
        }
    }
}
