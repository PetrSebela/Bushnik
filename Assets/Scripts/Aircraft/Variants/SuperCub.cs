using Aircraft.Components;
using UnityEngine;

namespace Aircraft.Variants
{
    /// <summary>
    /// Controller for supercub bushplane
    /// </summary>
    public class SuperCub : Aircraft
    {
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
        /// Left flap 
        /// </summary>
        public Flap leftFlap;
        
        /// <summary>
        /// Right flap
        /// </summary>
        public Flap rightFlap;
        
        /// <summary>
        /// Engine
        /// </summary>
        public Engine engine;

        public override float EngineSpeed => engine.EngineSpeed;
        
        public override float EngineThrottle => engine.Throttle;

        public override void SetThrottleInput(float input)
        {
            engine.SetThrottle(input);
        }
        
        public override void SetPitchInput(float input)
        {
            elevator.SetInput(-input);
        }
        
        public override void SetRollInput(float input)
        {
            left.SetInput(-input);
            right.SetInput(input);
        }

        public override void SetYawInput(float input)
        {
            tailwheel.SetSteerInput(-input);
            rudder.SetInput(input);
        }
        
        public override void SetBrakeInput(float leftBrake, float rightBrake)
        {
            leftGear.SetBrakeInput(leftBrake);
            rightGear.SetBrakeInput(rightBrake);
        }
        
        public override void SetFlapAngle(float angle)
        {
            leftFlap.SetAngle(angle);
            rightFlap.SetAngle(angle);
        }
    }
}
