using Game.World;
using UnityEngine;

namespace UI.HUD
{
    /// <summary>
    /// Class responsible for updating values of HUD elements
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        /// <summary>
        /// Main camera
        /// </summary>
        [SerializeField] private Transform _camera;
     
        /// <summary>
        /// Aircraft rigidbody
        /// </summary>
        private Rigidbody _aircraftBody;
        
        /// <summary>
        /// Aircraft object
        /// </summary>
        private Aircraft.Aircraft _aircraft;
        
        /// <summary>
        /// Compass HUD element
        /// </summary>
        [SerializeField] private Compass _compass;
        
        /// <summary>
        /// IAS indicator element
        /// </summary>
        [SerializeField] private Dial _speedDial;
        
        /// <summary>
        /// Altimeter element
        /// </summary>
        [SerializeField] private Tape _altimeter;
        
        /// <summary>
        /// Engine speed dial
        /// </summary>
        [SerializeField] private Dial _engineSpeed;
        
        /// <summary>
        /// Throttle display
        /// </summary>
        [SerializeField] private ThrottleDisplay _throttleDisplay;


        private void Start()
        {
            if (!GameManager.Instance)
            {
                UnityEngine.Debug.Log("Using fallback objects");
                _aircraft = Utility.Generic.GetComponentInScene<Aircraft.Aircraft>();
                _aircraftBody = _aircraft.GetComponent<Rigidbody>();
                return;
            }
            
            _aircraft = GameManager.Instance.Aircraft;
            _aircraftBody = GameManager.Instance.AircraftRigidbody;
        }
        
        /// <summary>
        /// Updates values of individual HUD elements
        /// </summary>
        private void Update()
        {
            var cameraDirection = Vector3.ProjectOnPlane(_camera.forward, Vector3.up).normalized;
            var heading = Vector3.SignedAngle(Vector3.forward, cameraDirection, Vector3.up);
            _compass.degrees = heading;
            
            _speedDial.SetValue(_aircraftBody.linearVelocity.magnitude);
            _altimeter.value = _aircraftBody.position.y;
            _engineSpeed.SetValue(_aircraft.EngineSpeed);
            _throttleDisplay.SetValue(_aircraft.EngineThrottle, _aircraft.Brake);
        }
    }
}
