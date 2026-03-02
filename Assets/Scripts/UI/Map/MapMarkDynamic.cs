using UnityEngine;
using UnityEngine.UI;

namespace UI.Map
{
    public class MapMarkDynamic : MonoBehaviour
    {
        public Transform Tracked;
        public Image Icon;
        private RectTransform rectTransform;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }
        
        void Update()
        {
            this.rectTransform.anchoredPosition = MapView.Instance.WorldToMapCoordinates(Tracked.position);
            var direction = Vector3.ProjectOnPlane(Tracked.forward, Vector3.up).normalized;
            var heading = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
            this.transform.rotation =  Quaternion.AngleAxis(heading, -Vector3.forward);
        }
    }
}
