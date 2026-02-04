using System.Collections.Generic;

namespace Aircraft.Airfoil
{
    /// <summary>
    /// Class containing real samples of airfoil performance data
    /// </summary>
    public class AirfoilData : Airfoil
    {
        /// <summary>
        /// Airfoil performance profile samples
        /// </summary>
        public List<AirfoilSample> profile = new ();

        /// <summary>
        /// Registered new sample to the airfoil model
        /// </summary>
        /// <param name="alpha">AOA</param>
        /// <param name="lift">Lift coefficient</param>
        /// <param name="drag">Drag coefficient</param>
        public void AddSample(float alpha, float lift, float drag)
        {
            AirfoilSample sample = new AirfoilSample(alpha, lift, drag);
            profile.Add(sample);
            profile.Sort((a,b ) => a.Alpha.CompareTo(b.Alpha));
        }
       
        /// <summary>
        /// Interpolates two samples to get values at desired position
        /// </summary>
        /// <param name="alpha">Angle of attack</param>
        /// <returns>Airfoil sample</returns>
        public override AirfoilSample GetSample(float alpha)
        {
            AirfoilSample low = null;
            AirfoilSample high = null;
            
            for (int i = 0; i < profile.Count - 1; i++)
            {
                var current =  profile[i];
                if (current.Alpha < alpha)
                    continue;
                
                low = profile[i];
                high = profile[i + 1];
                break;
            }
            
            if(low == null || high == null)
                return AirfoilSample.Zero;
            
            return AirfoilSample.Lerp(low, high, alpha);
        }
    }
}
