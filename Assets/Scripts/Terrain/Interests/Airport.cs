using UI.Map;
using UnityEngine;

namespace Terrain.Interests
{
    /// <summary>
    /// Script for managing single airport functionality
    /// </summary>
    public class Airport : MonoBehaviour
    {
        /// <summary>
        /// Creates and places airport object
        /// </summary>
        /// <param name="airportName">Airport name</param>
        /// <param name="position">Airport position</param>
        /// <returns>Object containing airport</returns>
        public static GameObject PlaceAirport(string airportName, Vector3 position)
        {
            GameObject airport = new GameObject("Airport");
            airport.transform.SetParent(TerrainFeatureManager.Instance.pointOfInterestParent);
            airport.transform.position = position;
            airport.name = airportName;
            airport.AddComponent<Airport>();
            MapMarkerUtility.Instance.PlaceTextMarker(airportName, airport.transform);
            return airport;
        }
    }
}
