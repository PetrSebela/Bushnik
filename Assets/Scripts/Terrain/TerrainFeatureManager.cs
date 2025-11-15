using System;
using Terrain.Data;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Terrain
{
    public class TerrainFeatureManager : MonoBehaviour
    {
        [SerializeField] private int seed = 0;
        
        [Tooltip("Number of tested runway orientations")]
        [SerializeField] private int RunwayOrientationSamples;
        
        [Tooltip("Number of runways")]
        [SerializeField] private int RunwayCount;

        [SerializeField] private int OversampleRatio;
        
        public static TerrainFeatureManager Instance => _instance;
        private static TerrainFeatureManager _instance;
        
        private RunwayData[] _runways;
        
        /// <summary>
        /// Initialize singleton instance
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                throw new Exception("Duplicate instance of TerrainFeatureManager");
        }
        
        public void Start()
        {
            _runways = GetRunways();
        }

        /// <summary>
        /// Generates runways on terrain
        /// </summary>
        /// <returns>Array of valid runways</returns>
        public RunwayData[] GetRunways()
        {
            Random.InitState(seed);
            
            int totalPointCount = 2 * RunwayOrientationSamples * RunwayCount;
            
            // Calculate all possible runway control points
            Vector3[] points = new Vector3[totalPointCount];

            for (int airportIndex = 0; airportIndex < RunwayCount; airportIndex++)
            {
                for (int runwayIndex = 0; runwayIndex < RunwayOrientationSamples; runwayIndex += 2)
                {
                    //TODO: Better dependency management for terrain values from different sources
                    Vector2 position = Random.insideUnitCircle;
                    Vector3 centerPoint = new Vector3(position.x, 0, position.y) * 40000;
                    
                    float heading = (float)runwayIndex / RunwayOrientationSamples * 360f;

                    float runwayLength = 1000;

                    Vector3 direction = Quaternion.AngleAxis(heading, Vector3.up) * Vector3.forward;

                    Vector3 approachThreshold = direction * (runwayLength / 2);
                    Vector3 departureThreshold = direction * -(runwayLength / 2);

                    int thresholdIndex = airportIndex * RunwayOrientationSamples + runwayIndex;
                    points[thresholdIndex] = centerPoint + approachThreshold;
                    points[thresholdIndex + 1] = centerPoint + departureThreshold;
                }
            }

            ComputeProxy.Instance.SamplePoints(ref points);
            
            RunwayData[] runways = new RunwayData[RunwayCount];
            
            for (int airportIndex = 0; airportIndex < RunwayCount; airportIndex++)
            {
                float bestRunwaySlope = 90f;
                RunwayData runway = new();
                
                for (int runwayIndex = 0; runwayIndex < RunwayOrientationSamples; runwayIndex += 2)
                {
                    int thresholdIndex = airportIndex * RunwayOrientationSamples + runwayIndex;
                    Vector3 approachThreshold = points[thresholdIndex];
                    Vector3 departureThreshold = points[thresholdIndex + 1];

                    Vector3 delta = departureThreshold - approachThreshold;
                    float angle = Mathf.Abs(90 - Vector3.Angle(Vector3.up, delta));

                    if (angle > bestRunwaySlope)
                        continue;

                    bestRunwaySlope = angle;
                    runway.ApproachThreshold = approachThreshold;
                    runway.DepartureThreshold = departureThreshold;
                }
                
                runways[airportIndex] = runway;
            }

            return runways;
        }

        public void OnDrawGizmos()
        {
            if(_runways == null)
                return;
            
            foreach (var runway in _runways)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(runway.ApproachThreshold, 50);
                Gizmos.DrawSphere(runway.DepartureThreshold, 50);
                Gizmos.DrawLine(runway.ApproachThreshold, runway.DepartureThreshold);
            }
        }
    }
}
