using System.Collections.Generic;
using Game.Mission;
using UI.Interactable;
using UI.Map;
using UnityEngine;

namespace Terrain.Interests
{
    /// <summary>
    /// Script for managing single airport functionality
    /// </summary>
    public class Airport : PointOfInterest
    {
        public void Init(string airportName, Vector3 position, List<MissionTemplate> missions)
        {
            transform.SetParent(TerrainFeatureManager.Instance.pointOfInterestParent);
            transform.position = position;
            name = airportName;
            Missions.AddRange(missions);
            MapMarkerUtility.Instance.PlaceTextMarker(airportName, transform);
        }
    }
}
