using System;
using Game.World;
using UI;
using UI.Map;
using UnityEngine;

namespace Terrain.Interests
{
    /// <summary>
    /// Script for managing single airport functionality
    /// </summary>
    public class Airport : PointOfInterest
    {
        public void Init(string airportName, Vector3 position)
        {
            transform.SetParent(TerrainFeatureManager.Instance.pointOfInterestParent);
            transform.position = position;
            name = airportName;

            MapMarkerUtility.Instance.PlaceTextMarker(airportName, transform);
        }
    }
}
