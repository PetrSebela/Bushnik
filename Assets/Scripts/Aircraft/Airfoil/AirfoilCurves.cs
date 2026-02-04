using UnityEngine;

namespace Aircraft.Airfoil
{
    /// <summary>
    /// Class representing airfoil aerodynamic coefficients with animation curves
    /// </summary>
    [CreateAssetMenu(fileName = "Airfoil/Airfoil", menuName = "Airfoil/Airfoil", order = 3)]
    public class AirfoilCurves : Airfoil
    {
        /// <summary>
        /// Curve with lift coefficients
        /// </summary>
        public AnimationCurve liftCurve;
        
        /// <summary>
        /// Curve with drag coefficients
        /// </summary>
        public AnimationCurve dragCurve;
        
        /// <summary>
        /// Returns airfoil sample
        /// </summary>
        /// <param name="alpha">Sample angle of attack</param>
        /// <returns>Sample</returns>
        public override AirfoilSample GetSample(float alpha)
        {
            var lift = liftCurve.Evaluate(alpha);
            var drag = dragCurve.Evaluate(alpha);
            return new AirfoilSample(alpha,  lift, drag);
        }
    }
}
