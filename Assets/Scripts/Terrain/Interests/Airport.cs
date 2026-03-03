using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Interests
{
    [CreateAssetMenu(fileName = "Airport", menuName = "POIs/Airport", order = 0)]
    public class Airport : PointOfInterestDescriptor
    {
        [SerializeField] private int count;
        
        private bool TryAddRunway(ref List<PointOfInterest> runways, Vector3 approach, float heading)
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

            var airport = new GameObject("Airport");
            airport.transform.position = approachPoint;
            airport.transform.SetParent(TerrainFeatureManager.Instance.PointOfInterestParent);
            var poi = airport.AddComponent<PointOfInterest>();
            
            var affector = new TerrainAffectorData
            {
                From = approachPoint,
                To = departurePoint,
                Width = 10f
            };
            
            poi.TerrainAffectors.Add(affector);
            runways.Add(poi);
            return true;
        }
        
        public override List<PointOfInterest> GetPointOfInterest()
        {
            List<PointOfInterest> airports = new();

            while (airports.Count < count)
            {
                int sampleCount = 12;
                int baseIndex = Random.Range(0, sampleCount);
                for (int orientationIndex = baseIndex; orientationIndex < sampleCount; orientationIndex++)
                {
                    Vector2 position = Random.insideUnitCircle;
                    Vector3 centerPoint = new Vector3(position.x, 0, position.y) * 40000;
                    float heading = (float)(orientationIndex % sampleCount) / sampleCount * 360f;
                    
                    if (TryAddRunway(ref airports, centerPoint, heading))
                        break;
                }
            }
            
            return airports;
        }
    }
}
