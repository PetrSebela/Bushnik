using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        

        private RunwayData[] _runways;
        public RunwayData[] Runways => _runways;
        
        public static TerrainFeatureManager Instance => _instance;
        
        private static TerrainFeatureManager _instance;
        
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

        private bool TryAddRunway(ref List<RunwayData> runways, Vector3 approach, float heading)
        {
            const int runwaySamples = 10;
            Vector3[] points = new Vector3[runwaySamples];
            
            Vector3 headingVector = Quaternion.AngleAxis(heading, Vector3.up) * Vector3.forward;
            float length = 250f;

            for (int sampleIndex = 0; sampleIndex < runwaySamples; sampleIndex++)
            {
                float progress = sampleIndex / (float)runwaySamples;
                var samplePosition = approach + headingVector * length * progress;
                points[sampleIndex] = samplePosition;
            }

            ComputeProxy.Instance.SamplePoints(ref points);
            
            var approachPoint = points[0];
            var departurePoint = points[^1];
            
            var flow = departurePoint - approachPoint;
            var angle = Vector3.Angle(Vector3.up, flow);

            // Limit runway slope
            if (Mathf.Abs(angle - 90) > 5f)
                return false;
            
            var totalDiff = 0f;
            for (int sampleIndex = 0; sampleIndex < runwaySamples; sampleIndex++)
            {
                float progress = sampleIndex / (float)runwaySamples;
                var flatSample = Vector3.Lerp(approachPoint, departurePoint, progress);
            
                var diff = flatSample.y - points[sampleIndex].y;
                totalDiff += Mathf.Abs(diff);
            }

            if (totalDiff / runwaySamples > 1)
                return false;
            
            var runway = new RunwayData
            {
                ApproachThreshold = approachPoint,
                DepartureThreshold = departurePoint,
                Width = 10f
            };
            runways.Add(runway);

            return true;
        }

        /// <summary>
        /// Generates runways on terrain
        /// </summary>
        /// <returns>Array of valid runways</returns>
        public void GetRunways(Action<RunwayData[]> onCompleted)
        {
            Random.InitState(seed);

            List<RunwayData> runways = new List<RunwayData>();
            
            for (int airportIndex = 0; airportIndex < RunwayCount; airportIndex++)
            {
                for (int runwayIndex = 0; runwayIndex < RunwayOrientationSamples; runwayIndex += 2)
                {
                    Vector2 position = Random.insideUnitCircle;
                    Vector3 centerPoint = new Vector3(position.x, 0, position.y) * 40000;
                    float heading = (float)runwayIndex / RunwayOrientationSamples * 360f;

                    if (TryAddRunway(ref runways, centerPoint, heading))
                        break;
                }
            }
            
            Debug.Log($"Generated {runways.Count} runways");
            _runways = runways.ToArray();
            onCompleted.Invoke(runways.ToArray());
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
