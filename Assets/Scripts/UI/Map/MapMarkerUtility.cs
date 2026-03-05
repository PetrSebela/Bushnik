using TMPro;
using UnityEngine;
using Utility;

namespace UI.Map
{
    /// <summary>
    /// Utility for placing markers onto map view
    /// </summary>
    public class MapMarkerUtility : Singleton<MapMarkerUtility>
    {
        /// <summary>
        /// Prefab for markers containing text
        /// </summary>
        [SerializeField] private GameObject textMarkerPrefab;
        
        /// <summary>
        /// Player marker prefab
        /// </summary>
        [SerializeField] private GameObject playerMarkerPrefab;

        /// <summary>
        /// Places marker with text onto the map
        /// </summary>
        /// <param name="text">Marker text</param>
        /// <param name="target">Marker position</param>
        public void PlaceTextMarker(string text, Transform target)
        {
            var marker = Instantiate(textMarkerPrefab, transform);
            marker.GetComponentInChildren<TMP_Text>().text = text;
            MapView.Instance.RegisterMarker(target, marker.GetComponent<RectTransform>());
        }

        /// <summary>
        /// Places player marker onto the map
        /// </summary>
        /// <param name="target">Object which will be tracked</param>
        public void PlacePlayerMarker(Transform target)
        {
            var marker = Instantiate(playerMarkerPrefab, transform);
            MapView.Instance.RegisterMarker(target, marker.GetComponent<RectTransform>());
        }
    }
}
