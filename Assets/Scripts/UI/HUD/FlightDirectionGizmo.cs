using Aircraft.Controller;
using UnityEngine;

namespace UI.HUD
{
    public class FlightDirectionGizmos : MonoBehaviour
    {
        [SerializeField] private AircraftController controller;
        [SerializeField] private RectTransform desiredDirectionGizmo;
        [SerializeField] private RectTransform currentDirectionGizmo;
        
        [SerializeField] private Camera canvasCamera;
        
        private void SetDirectionGizmos(RectTransform rect, Vector3 direction)
        {
            rect.gameObject.SetActive(!(Vector3.Dot(direction, canvasCamera.transform.forward) < 0));
            
            var viewportPoint = canvasCamera.WorldToViewportPoint(canvasCamera.transform.position + direction) - new Vector3(0.5f, 0.5f, 0);
            rect.anchoredPosition = new Vector2(viewportPoint.x * 1920, viewportPoint.y * 1080);
        }

        void LateUpdate()
        {
            SetDirectionGizmos(currentDirectionGizmo, controller.transform.forward);
        }
    }
}
