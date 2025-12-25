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
        [SerializeField] private Transform cameraTransform;
        /// <summary>
        /// Normal camera speed
        /// </summary>
        [Header("Movement")]
        [SerializeField] private float cameraSpeed = 10;
        
        /// <summary>
        /// Boosted movement speed
        /// </summary>
        [SerializeField] private float cameraBoostSpeed = 100;
        
        /// <summary>
        /// Sensitivity for camera rotation
        /// </summary>
        [Header("Mouse")]
        [SerializeField] private float mouseSensitivity = 20;
        
        /// <summary>
        /// Camera sensitivity for joystick
        /// </summary>
        [Header("Controller")]
        [SerializeField] private float joystickSensitivity = 20;

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
        private Vector2 _cameraJoystickInput = Vector2.zero;
        
        /// <summary>
        /// Flag for boosted movement speed
        /// </summary>
        private bool _boosted = false;

        /// <summary>
        /// Camera rotation smooth factor
        /// </summary>
        [SerializeField] private float smoothFactor = 1;

        /// <summary>
        /// Smoothed camera rotation input
        /// </summary>
        private Vector2 _smoothInput = Vector2.zero;

        /// <summary>
        /// If camera is zoomed in
        /// </summary>
        private bool _zoomed = false;

        
        /// <summary>
        /// Unzoomed camera FOV
        /// </summary>
        [Header("Camera")] 
        [SerializeField] private float normalFOV = 90;
        
        /// <summary>
        /// Zoomed camera FOV
        /// </summary>
        [SerializeField] private float zoomedFOV = 25;
        
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
            _smoothInput = Vector2.Lerp(_smoothInput, _cameraJoystickInput, smoothFactor);
            
            _attitude += _smoothInput * joystickSensitivity;
            _attitude = new Vector2(_attitude.x, Mathf.Clamp(_attitude.y, -90, 90));

            var horizontalRotation = Quaternion.AngleAxis(_attitude.x, Vector3.up);

            var forward = horizontalRotation * Vector3.forward * _direction.x;
            var right = horizontalRotation * Vector3.right * _direction.z;
            var up = Vector3.up * _direction.y;
            
            float velocity = _boosted ? cameraBoostSpeed : cameraSpeed;
            var force = (forward + right + up) * velocity;
            cameraBody.AddForce(force, ForceMode.Acceleration);
        }

        private void Update()
        {
            var horizontalRotation = Quaternion.AngleAxis(_attitude.x, Vector3.up);
            var verticalRotation = Quaternion.AngleAxis(-_attitude.y, Vector3.right);
            var attitude = horizontalRotation * verticalRotation;
            
            cameraTransform.SetPositionAndRotation(cameraBody.position, attitude);
        }

        /// <summary>
        /// Returns angle from north
        /// </summary>
        public float GetCardinalDirection => _attitude.x;
        
        void OnCameraJoystickPerformed(InputAction.CallbackContext context)
        {
            var input = context.ReadValue<Vector2>();
            // Invert joystick pitch axis because of personal preference
            _cameraJoystickInput = new Vector3(input.x, -input.y);
        }

        void OnCameraJoystickCancelled(InputAction.CallbackContext context)
        {
            _cameraJoystickInput = Vector2.zero;
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

        void OnZoomPerformed(InputAction.CallbackContext context)
        {
            _zoomed = !_zoomed;
            Camera.main.fieldOfView = _zoomed ? zoomedFOV : normalFOV;
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
            InputProvider.Instance.Input.Demo.Zoom.performed += OnZoomPerformed;
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
            InputProvider.Instance.Input.Demo.Zoom.performed -= OnZoomPerformed;
        }
    }
}
