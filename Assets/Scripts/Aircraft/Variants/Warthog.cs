using Aircraft.Components;
using UnityEngine;

namespace Aircraft.Variants
{
    /// <summary>
    /// Controller for warthog aircraft
    /// </summary>
    public class Warthog : Aircraft
    {
        public Flap leftAileron;
        
        public Flap rightAileron;
        
        public Flap elevator;
        
        public Flap leftRudder;
        
        public Flap rightRudder;
        
        public Engine leftEngine;
        
        public Engine rightEngine;
        
        public Tailwheel noseWheel;

        public Shock leftLandingGear;
        
        public Shock rightLandingGear;
        
        public override float EngineSpeed => leftEngine.EngineSpeed;
        
        public override float EngineThrottle => leftEngine.Throttle;
        
        public override void SetThrottleInput(float input)
        {
            leftEngine.SetThrottle(input);
            rightEngine.SetThrottle(input);
        }
        
        public override void SetPitchInput(float input)
        {
            elevator.SetInput(-input);
        }
        
        public override void SetRollInput(float input)
        {
            leftAileron.SetInput(-input);
            rightAileron.SetInput(input);
        }

        public override void SetYawInput(float input)
        {
            noseWheel.SetSteerInput(-input);
            leftRudder.SetInput(input);
            rightRudder.SetInput(input);
        }
        
        public override void SetBrakeInput(float leftBrake, float rightBrake)
        {
            leftLandingGear.SetBrakeInput(leftBrake);
            rightLandingGear.SetBrakeInput(rightBrake);
        }
    }
}
