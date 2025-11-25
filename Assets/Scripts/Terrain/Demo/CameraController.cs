using System;
using UnityEngine;
using UnityEngine.InputSystem;
using User;

namespace Terrain.Demo
{
    /// <summary>
    /// Extremely basic camera controller that allows unrestricted movement around scene
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        /// <summary>
        /// Normal camera speed
        /// </summary>
        [SerializeField] private float cameraSpeed = 10;
        
        /// <summary>
        /// Boosted movement speed
        /// </summary>
        [SerializeField] private float cameraBoostSpeed = 100;
        
        /// <summary>
        /// Sensitivity for camera rotation
        /// </summary>
        [SerializeField] private float mouseSensitivity = 20;
        
        /// <summary>
        /// Camera rigidbody
        /// </summary>
        [SerializeField] private Rigidbody cameraBody;

        /// <summary>
        /// Camera attitude in degrees
        /// </summary>
        private Vector2 _attitude = Vector2.zero;
        
        /// <summary>
        /// Camera direction vector
        /// </summary>
        private Vector3 _direction = Vector3.zero;
        
        /// <summary>
        /// Camera input vector in local space
        /// </summary>
        private Vector2 _cameraInput = Vector2.zero;
        
        /// <summary>
        /// Flag for boosted movement speed
        /// </summary>
        private bool _boosted = false;
        
        
        /// <summary>
        /// Initialization
        /// </summary>
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            RegisterInput();
        }

        /// <summary>
        /// Updating camera position and rotation
        /// </summary>
        void FixedUpdate()
        {
            _attitude += _cameraInput * 3;
            _attitude = new Vector2(_attitude.x, Mathf.Clamp(_attitude.y, -90, 90));

            var horizontalRotation = Quaternion.AngleAxis(_attitude.x, Vector3.up);
            var verticalRotation = Quaternion.AngleAxis(-_attitude.y, Vector3.right);

            var forward = horizontalRotation * Vector3.forward * _direction.x;
            var right = horizontalRotation * Vector3.right * _direction.z;
            var up = Vector3.up * _direction.y;

            var attitude = horizontalRotation * verticalRotation;
            cameraBody.rotation = attitude;
            
            float velocity = _boosted ? cameraBoostSpeed : cameraSpeed;
            cameraBody.linearVelocity = velocity * (forward + right + up);
        }

        void OnCameraJoystickPerformed(InputAction.CallbackContext context)
        {
            var input = context.ReadValue<Vector2>();
            // Invert joystick pitch axis because of personal preference
            _cameraInput = new Vector3(input.x, -input.y);
        }

        void OnCameraJoystickCancelled(InputAction.CallbackContext context)
        {
            _cameraInput = Vector2.zero;
        }

        void OnMovementPerformed(InputAction.CallbackContext context)
        {
            var desiredDirection = context.ReadValue<Vector2>();
            _direction = new Vector3(desiredDirection.y, _direction.y, desiredDirection.x);
        }

        void OnMovementCancelled(InputAction.CallbackContext context)
        {
            _direction = new Vector3(0, _direction.y, 0);
        }

        void OnCameraMousePerformed(InputAction.CallbackContext context)
        {
            var delta = context.ReadValue<Vector2>();
            _attitude += delta * mouseSensitivity;
        }

        void OnAltitudePerformed(InputAction.CallbackContext context)
        {
            float direction = context.ReadValue<float>();
            _direction = new Vector3(_direction.x, direction, _direction.z);
        }

        void OnAltitudeCancelled(InputAction.CallbackContext context)
        {
            _direction = new Vector3(_direction.x, 0, _direction.z);
        }

        void OnBoostPerformed(InputAction.CallbackContext context)
        {
            _boosted = !_boosted;
        }


        private void RegisterInput()
        {
            InputProvider.Instance.Input.Demo.CameraJoystick.performed += OnCameraJoystickPerformed;
            InputProvider.Instance.Input.Demo.CameraJoystick.canceled += OnCameraJoystickCancelled;
            InputProvider.Instance.Input.Demo.CameraMouse.performed += OnCameraMousePerformed;
            InputProvider.Instance.Input.Demo.Movement.performed += OnMovementPerformed;
            InputProvider.Instance.Input.Demo.Movement.canceled += OnMovementCancelled;
            InputProvider.Instance.Input.Demo.Vertical.performed += OnAltitudePerformed;
            InputProvider.Instance.Input.Demo.Vertical.canceled += OnAltitudeCancelled;
            InputProvider.Instance.Input.Demo.Boost.performed += OnBoostPerformed;
        }

        private void OnDisable()
        {
            InputProvider.Instance.Input.Demo.CameraJoystick.performed -= OnCameraJoystickPerformed;
            InputProvider.Instance.Input.Demo.CameraJoystick.canceled -= OnCameraJoystickCancelled;
            InputProvider.Instance.Input.Demo.CameraMouse.performed -= OnCameraMousePerformed;
            InputProvider.Instance.Input.Demo.Movement.performed -= OnMovementPerformed;
            InputProvider.Instance.Input.Demo.Vertical.canceled -= OnMovementCancelled;
            InputProvider.Instance.Input.Demo.Vertical.performed -= OnAltitudePerformed;
            InputProvider.Instance.Input.Demo.Vertical.canceled -= OnAltitudeCancelled;
            InputProvider.Instance.Input.Demo.Boost.performed -= OnBoostPerformed;
        }
    }
}
