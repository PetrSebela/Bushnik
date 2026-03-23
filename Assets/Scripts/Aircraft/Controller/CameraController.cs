using Game.World;
using UnityEngine;
using UnityEngine.InputSystem;
using User;

namespace Aircraft.Controller
{
    /// <summary>
    /// Camera controller for when the player uses external aircraft camera
    /// </summary>
    public class CameraController : MonoBehaviour
    { 
        /// <summary>
        /// Camera direction follow smoothing factor
        /// </summary>
        [SerializeField] private float followSmoothing = 10f;
        
        /// <summary>
        /// Mouse sensitivity
        /// </summary>
        [SerializeField] private float mouseSensitivity = 10f;
        
        /// <summary>
        /// After what time of inactivity the offset should be canceled
        /// </summary>
        [SerializeField] private float alightAfter = 1f;
        
        [SerializeField] private Transform cameraTransform;
        
        /// <summary>
        /// Reference to related aircraft controller
        /// </summary>
        private Aircraft _tracked;
        
        /// <summary>
        /// Targeted offset 
        /// </summary>
        private Vector3 _targetOffset;
        
        /// <summary>
        /// Real camera orientation
        /// </summary>
        private Quaternion _rotation;

        /// <summary>
        /// Last time the offset was changed
        /// </summary>
        private double _lastOffsetChangeTime;

        /// <summary>
        /// If the offset is active
        /// </summary>
        private bool _offsetActive = false;
        
        /// <summary>
        /// If the offset is active
        /// </summary>
        private bool OffsetActive => _offsetActive;
        
        void Start()
        {
            RegisterInput();

            if (!GameManager.Instance)
            {
                Debug.Log("Using fallback AC");
                _tracked = Utility.Generic.GetComponentInScene<Aircraft>();
            }
            else
                _tracked = GameManager.Instance.Aircraft;
            
            cameraTransform.localPosition = _tracked.cameraOffset;
        }
        
        /// <summary>
        /// If offset should be used
        /// </summary>
        /// <returns>true if offset should be applied, false otherwise</returns>
        bool GetOffsetActive()
        {
            if(_tracked.Velocity < 1f)
                return true;
            
            if(_offsetActive && Time.realtimeSinceStartupAsDouble - _lastOffsetChangeTime > alightAfter)
                return false;

            return _offsetActive;
        }
        
        void Update()
        {
            _offsetActive = GetOffsetActive();
            
            if (_offsetActive)
            {
                _rotation = Quaternion.Euler(_targetOffset);;
            }
            else
            {
                var desired = Quaternion.LookRotation( _tracked.transform.forward, Vector3.up);
                _rotation = Quaternion.Slerp(_rotation, desired, followSmoothing * Time.deltaTime);
                _rotation = Quaternion.LookRotation(_rotation * Vector3.forward, Vector3.up); // Keeps camera horizon aligned at all times
                _targetOffset = _rotation.eulerAngles; // Offset is related to the original follow direction
            }
            transform.rotation = _rotation;
            transform.position = _tracked.transform.position;
        }

        /// <summary>
        /// Forces camera to be aligned with direction of travel
        /// </summary>
        public void CenterCamera()
        {
            _offsetActive = false;
        }
        
        /// <summary>
        /// Processes mouse input
        /// </summary>
        /// <param name="context">mouse input context</param>
        void OnMouseLookPerformed(InputAction.CallbackContext context)
        {
            var input = context.ReadValue<Vector2>();
            _targetOffset += new Vector3(-input.y, input.x, 0) * mouseSensitivity;
            
            var baseOffset = Mathf.Round(_targetOffset.x / 360) * 360f;
            _targetOffset.x = Mathf.Clamp(_targetOffset.x, baseOffset - 89, baseOffset + 89);
            
            _offsetActive = true;
            _lastOffsetChangeTime = Time.realtimeSinceStartupAsDouble;
        }
        
        void RegisterInput()
        {
            InputProvider.Instance.Input.Aircraft.Look.performed += OnMouseLookPerformed;
        }

        void OnDisable()
        {
            InputProvider.Instance.Input.Aircraft.Look.performed -= OnMouseLookPerformed;   
        }
    }
}
