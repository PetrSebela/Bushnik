using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Terrain.Demo
{
    /// <summary>
    /// Extremely basic camera controller that allows unrestricted movement around scene
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float cameraSpeed = 10;
        [SerializeField] private float mouseSensitivity = 20;

        private Vector2 _attitude = Vector2.zero;
        private Vector3 _direction = Vector3.zero;
        private Vector2 _cameraInput = Vector2.zero;
        
        private SimInput _input;

        private void Awake()
        {
            _input = new SimInput();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        
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
            transform.rotation = attitude;
            transform.position += Time.fixedDeltaTime * cameraSpeed * (forward + right + up);
        }

        void OnCameraJoystickPerformed(InputAction.CallbackContext context)
        {
            var input = context.ReadValue<Vector2>();
            // Invert tilt axis because of personal preference
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
        
        private void OnEnable()
        {
            _input.Demo.CameraJoystick.performed += OnCameraJoystickPerformed;
            _input.Demo.CameraJoystick.canceled += OnCameraJoystickCancelled;
            _input.Demo.CameraMouse.performed += OnCameraMousePerformed;
            _input.Demo.Movement.performed += OnMovementPerformed;
            _input.Demo.Movement.canceled += OnMovementCancelled;
            _input.Demo.Vertical.performed += OnAltitudePerformed;
            _input.Demo.Vertical.canceled += OnAltitudeCancelled;
            _input.Enable();
        }

        private void OnDisable()
        {
            _input.Demo.CameraJoystick.performed -= OnCameraJoystickPerformed;
            _input.Demo.CameraJoystick.canceled -= OnCameraJoystickCancelled;
            _input.Demo.CameraMouse.performed -= OnCameraMousePerformed;
            _input.Demo.Movement.performed -= OnMovementPerformed;
            _input.Demo.Vertical.canceled -= OnMovementCancelled;
            _input.Demo.Vertical.performed -= OnAltitudePerformed;
            _input.Demo.Vertical.canceled -= OnAltitudeCancelled;
            _input.Disable();
        }
    }
}
