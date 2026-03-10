using System.Collections.Generic;
using System.Linq;
using Game.Mission;
using UI.Map;
using UnityEngine;
using Utility;

namespace Terrain.Interests
{
    [CreateAssetMenu(fileName = "Airport", menuName = "POIs/Airport", order = 0)]
    public class AirportDescriptor : PointOfInterestDescriptor
    {
        [SerializeField] private string airportName;
        [SerializeField] private List<MissionTemplate> missions;
        [SerializeField] private float minSpacing;
        [SerializeField] private GameObject airportPrefab;
        [SerializeField] private GameObject hangarPrefab;
        [SerializeField] private GameObject windTurbinePrefab;
        
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
            var center = (approachPoint + departurePoint) / 2;
            
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

                if (points[sampleIndex].y < 10)
                    return false;
            }

            if (totalDiff / runwaySamples > 1)
                return false;
            
            var airport = Instantiate(airportPrefab).GetComponent<Airport>();
            airport.Init(airportName, center, missions);
            
            var affector = new TerrainAffectorData
            {
                From = approachPoint,
                To = departurePoint,
                Width = 10f
            };
            
            airport.TerrainAffectors.Add(affector);
            runways.Add(airport);

            var hangarRotation = Quaternion.AngleAxis(heading + 90, Vector3.up);
            var hangarPosition = approachPoint + hangarRotation * Vector3.forward * 35; 
            
            var hangar = Instantiate(hangarPrefab, airport.transform);
            hangar.transform.position = hangarPosition;
            hangar.transform.rotation = hangarRotation;
            
            var positions = RandomProvider.GetRandomPointsIn(center, 1000, 25);
            ComputeProxy.Instance.SamplePoints(ref positions);
            List<Vector3> occupied = new(){hangarPosition};
            
            foreach (var point in positions)
            {
                var relative = point - approachPoint;
                var headingDirection = departurePoint - approachPoint;
                
                var projected = Vector3.Dot(relative, headingDirection) / headingDirection.sqrMagnitude;
                
                if(Vector3.Distance(approachPoint + headingDirection * projected, point) < 100)
                    continue;

                if (occupied.Any(taken => Vector3.Distance(taken, point) < 25))
                    continue;
                
                occupied.Add(point);
                var turbine = Instantiate(windTurbinePrefab, airport.transform);
                turbine.transform.position = point;
            }
            
            return true;
        }
        
        public override List<PointOfInterest> GetPointOfInterest()
        {
            List<PointOfInterest> airports = new();

            while (true)
            {
                int sampleCount = 12;
                int baseIndex = Random.Range(0, sampleCount);
                
                Vector2 position = Random.insideUnitCircle;
                Vector3 centerPoint = new Vector3(position.x, 0, position.y) * TerrainManager.Instance.terrainSettings.size / 2f;
                
                if(TerrainFeatureManager.Instance.Interests.Any(airport => Vector3.Distance(centerPoint, airport.transform.position) <= minSpacing))
                    continue;
                
                for (int orientationIndex = baseIndex; orientationIndex < sampleCount; orientationIndex++)
                {
                    float heading = (float)(orientationIndex % sampleCount) / sampleCount * 360f;
                    if (TryAddRunway(ref airports, centerPoint, heading))
                        return airports;
                }
            }
        }
    }
}
