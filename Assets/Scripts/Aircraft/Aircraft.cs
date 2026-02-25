using System.Linq;
using Aircraft.Components;
using UnityEngine;

namespace Aircraft
{
    /// <summary>
    /// Interface between aircraft controller and individual aircraft components
    /// </summary>
    public class Aircraft : MonoBehaviour
    {
        /// <summary>
        /// Reference to plane rigidbody component
        /// </summary>
        [SerializeField] private Rigidbody aircraftBody;
        
        /// <summary>
        /// Aircraft tail wheel
        /// </summary>
        public Tailwheel tailwheel;

        /// <summary>
        /// Left front landing gear
        /// </summary>
        public Shock leftGear;
        
        /// <summary>
        /// Left front landing gear
        /// </summary>
        public Shock rightGear;
        
        /// <summary>
        /// Left aileron
        /// </summary>
        public Flap left;
        
        /// <summary>
        /// Right aileron
        /// </summary>
        public Flap right;
        
        /// <summary>
        /// Elevator
        /// </summary>
        public Flap elevator;
        
        /// <summary>
        /// Rudder
        /// </summary>
        public Flap rudder;
        
        /// <summary>
        /// Engine
        /// </summary>
        public Engine engine;

        /// <summary>
        /// Aircraft velocity in knots
        /// </summary>
        public float VelocityKnots => aircraftBody.linearVelocity.magnitude * 1.9438445f;

        /// <summary>
        /// Aircraft velocity in mps
        /// </summary>
        public float Velocity => aircraftBody.linearVelocity.magnitude;
        
        public Quaternion AngularVelocity => Quaternion.Euler(aircraftBody.angularVelocity);
        
        /// <summary>
        /// Sets engine throttle input
        /// </summary>
        /// <param name="input">Surface deflection (-1, 1)</param>
        public void SetThrottleInput(float input)
        {
            engine.SetThrottle(input);
        }
        
        /// <summary>
        /// Sets pitch controls surface angle
        /// </summary>
        /// <param name="input">Surface deflection (-1, 1)</param>
        public void SetPitchInput(float input)
        {
            elevator.SetInput(-input);
        }
        
        /// <summary>
        /// Sets roll controls surface angle
        /// </summary>
        /// <param name="input">Surface deflection (-1, 1)</param>
        public void SetRollInput(float input)
        {
            left.SetInput(-input);
            right.SetInput(input);
        }

        /// <summary>
        /// Sets yaw controls surface angle
        /// </summary>
        /// <param name="input">Surface deflection (-1, 1)</param>
        public void SetYawInput(float input)
        {
            tailwheel.SetSteerInput(-input);
            rudder.SetInput(input);
        }

        /// <summary>
        /// Sets brake input for left and right wheels of front landing gear
        /// </summary>
        /// <param name="leftBrake">Left brake input</param>
        /// <param name="rightBrake">Right brake input</param>
        public void SetBrakeInput(float leftBrake, float rightBrake)
        {
            leftGear.SetBrakeInput(leftBrake);
            rightGear.SetBrakeInput(rightBrake);
        }
        
        /// <summary>
        /// Returns airfrafts average center of pressure during forward level flight
        /// </summary>
        private Vector3 GetAverageCenterOfPressure()
        {
            var wings = GetComponentsInChildren<Wing>();

            var totalArea = wings.Aggregate(0f, (current, wing) => current + wing.WingArea);
            var totalWeight = 0f;
            var centerOfPressure = Vector3.zero;
            foreach (var wing in  wings)
            {
                var liftComponent = Mathf.Clamp01(Vector3.Dot(Vector3.up, wing.transform.up));
                var weight = wing.WingArea / totalArea * liftComponent;
                totalWeight += weight;
                centerOfPressure += wing.transform.position * weight;
            }
            centerOfPressure /= totalWeight;
            return centerOfPressure;
        }
        
        /// <summary>
        /// Draws basic debug gizmos (CG - yellow, CP - blue)
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(aircraftBody.worldCenterOfMass, 0.125f);
            
            var cop = GetAverageCenterOfPressure();
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(cop, 0.125f);
        }
    }
}
