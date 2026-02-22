using UnityEngine;

namespace UI.HUD
{
    public class HUDManager : MonoBehaviour
    {
        [SerializeField] private Transform _camera;
        [SerializeField] private Rigidbody _aircraftBody;
        [SerializeField] private Aircraft.Aircraft _aircraft;
        [SerializeField] private Compass _compass;
        [SerializeField] private Dial _speedDial;
        [SerializeField] private Tape _altimeter;
        [SerializeField] private Dial _engineSpeed;
        [SerializeField] private ThrottleDisplay _throttleDisplay;
        
        void Update()
        {
            var cameraDirection = Vector3.ProjectOnPlane(_camera.forward, Vector3.up).normalized;
            var heading = Vector3.SignedAngle(Vector3.forward, cameraDirection, Vector3.up);
            _compass.degrees = heading;
            _speedDial.SetValue(_aircraftBody.linearVelocity.magnitude);
            _altimeter.value = _aircraftBody.position.y;
            _engineSpeed.SetValue(_aircraft.engine.EngineSpeed);
            _throttleDisplay.SetValue(_aircraft.engine.Throttle);
        }
    }
}
