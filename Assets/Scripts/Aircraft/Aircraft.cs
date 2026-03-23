using System;
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
        /// Aircraft velocity in knots
        /// </summary>
        public float VelocityKnots => aircraftBody.linearVelocity.magnitude * 1.9438445f;

        /// <summary>
        /// Aircraft camera offset
        /// </summary>
        public Vector3 cameraOffset;
        
        /// <summary>
        /// Aircraft velocity in mps
        /// </summary>
        public float Velocity => aircraftBody.linearVelocity.magnitude;
        
        /// <summary>
        /// Aircraft angular velocity
        /// </summary>
        public Quaternion AngularVelocity => Quaternion.Euler(aircraftBody.angularVelocity);

        /// <summary>
        /// Sets engine throttle input
        /// </summary>
        /// <param name="input">Throttle percentage (0, 1)</param>
        public virtual void SetThrottleInput(float input) { }
        
        /// <summary>
        /// Sets pitch controls surface angle
        /// </summary>
        /// <param name="input">Surface deflection (-1, 1)</param>
        public virtual void SetPitchInput(float input) { }

        /// <summary>
        /// Sets roll controls surface angle
        /// </summary>
        /// <param name="input">Surface deflection (-1, 1)</param>
        public virtual void SetRollInput(float input) { }

        /// <summary>
        /// Sets yaw controls surface angle
        /// </summary>
        /// <param name="input">Surface deflection (-1, 1)</param>
        public virtual void SetYawInput(float input) { }

        /// <summary>
        /// Aircraft engine speed
        /// </summary>
        public virtual float EngineSpeed => 0f;
        
        /// <summary>
        /// Aircraft engine throttle
        /// </summary>
        public virtual float EngineThrottle => 0f;
        
        /// <summary>
        /// Current brake percentage
        /// </summary>
        public virtual float Brake => 0f;
        
        /// <summary>
        /// Sets brake input for left and right wheels of front landing gear
        /// </summary>
        /// <param name="leftBrake">Left brake input</param>
        /// <param name="rightBrake">Right brake input</param>
        public virtual void SetBrakeInput(float leftBrake, float rightBrake) { }
        
        /// <summary>
        /// Sets flap angle
        /// </summary>
        /// <param name="angle">Angle of flaps</param>
        public virtual void SetFlapAngle(float angle) { }
        
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
