using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace Aircraft.Airfoil
{
    /// <summary>
    /// Abstract class representing basic API for airfoil models
    /// </summary>
    public abstract class Airfoil : ScriptableObject
    {
        /// <summary>
        /// Returns airfoil coefficients at desired AOA
        /// </summary>
        /// <param name="alpha">Angle of attack</param>
        /// <returns>Aerodynamic coefficients</returns>
        public abstract AirfoilSample GetSample(float alpha);
    }


    /// <summary>
    /// Airfoil performance profile sample
    /// </summary>
    [System.Serializable]
    public class AirfoilSample
    { 
        /// <summary>
        /// Sample angle of attack
        /// </summary>
        public float Alpha;
        
        /// <summary>
        /// Sample lift coefficient
        /// </summary>
        public float Lift;
        
        /// <summary>
        /// Sample lift coefficient
        /// </summary>
        public float Drag;

        /// <summary>
        /// Sample with zero values
        /// </summary>
        public static AirfoilSample Zero => new (0,0,0);
        
        /// <summary>
        /// Default sample constructor
        /// </summary>
        /// <param name="alpha">alpha</param>
        /// <param name="lift">lift coefficient</param>
        /// <param name="drag">drag coefficient</param>
        public AirfoilSample(float alpha, float lift, float drag)
        {
            Alpha = alpha;
            Lift = lift;
            Drag = drag;
        }
        
        /// <summary>
        /// Returns string representation of instance
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            return $"alpha={this.Alpha}, lift={this.Lift}, drag={this.Drag}";
        }

        /// <summary>
        /// Interpolated two samples based on angle
        /// </summary>
        /// <param name="a">lower sample</param>
        /// <param name="b">upper sample</param>
        /// <param name="desiredAngle">desired angle for interpolation</param>
        /// <returns></returns>
        public static AirfoilSample Lerp(AirfoilSample a, AirfoilSample b, float desiredAngle)
        {
            if(Mathf.Approximately(a.Alpha, b.Alpha))
                return a;
            
            var t = (desiredAngle -  a.Alpha) / (b.Alpha - a.Alpha);
            
            var alpha = a.Alpha + (b.Alpha - a.Alpha) * t;
            var lift = a.Lift + (b.Lift - a.Lift) * t;
            var drag = a.Drag + (b.Drag - a.Drag) * t;
            return new AirfoilSample(alpha, lift, drag);
        }
    }
}
