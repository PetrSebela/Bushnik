using UnityEngine;
using UnityEngine.UI;

namespace UI.Map
{
    public class MapMarkStatic : MonoBehaviour
    {
        public Vector3 WorldPosition;
        public Image Icon;

        private RectTransform rectTransform;


        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }
        
        void Update()
        {
            this.rectTransform.anchoredPosition = MapView.Instance.WorldToMapCoordinates(WorldPosition);
        }
    }
}
