using System;
using System.Collections.Generic;
using Terrain;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using User;

namespace UI.Map
{
    /// <summary>
    /// Component responsible for interaction with map UI
    /// </summary>
    public class MapView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// Canvas used for map rendering
        /// </summary>
        [SerializeField] private Canvas mapCanvas;
        
        /// <summary>
        /// Map image component
        /// </summary>
        [SerializeField] private RawImage mapImage;
        
        /// <summary>
        /// Map resolution
        /// </summary>
        [SerializeField] private int mapResolution;
        
        /// <summary>
        /// Transform of object with map image
        /// </summary>
        [SerializeField] private RectTransform mapTransform;
        
        /// <summary>
        /// Minimum map scale
        /// </summary>
        [SerializeField] private float minScale;
        
        /// <summary>
        /// Maximum map scale
        /// </summary>
        [SerializeField] private float maxScale;
        
        /// <summary>
        /// Scale slider
        /// </summary>
        [SerializeField] private Slider scaleSlider;
        
        /// <summary>
        /// If user is dragging the map
        /// </summary>
        private bool _isDragging = false;
        
        /// <summary>
        /// Current map scale
        /// </summary>
        private float _scale = 1;
        
        /// <summary>
        /// Map view singleton
        /// </summary>
        private static MapView _instance;
        
        /// <summary>
        /// Singleton getter
        /// </summary>
        public static MapView Instance => _instance;

        /// <summary>
        /// If mouse is over map
        /// </summary>
        private bool _isMouseOver = false;
        
        /// <summary>
        /// List of all marker pairs
        /// </summary>
        private readonly List<MarkerPair> _markers = new();

        /// <summary>
        /// Last mouse position
        /// </summary>
        private Vector2 _lastMousePosition = Vector2.zero;
        
        /// <summary>
        /// Struct containing data used for rendering map markers
        /// </summary>
        private struct MarkerPair
        {
            public Transform Tracked;
            public RectTransform Marker;
        }
        
        private void Awake()
        {
            _instance = this;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isMouseOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isMouseOver = false;
        }

        /// <summary>
        /// Registers map marker
        /// </summary>
        /// <param name="tracked"></param>
        /// <param name="marker"></param>
        public void RegisterMarker(Transform tracked, RectTransform marker)
        {
            var pair = new MarkerPair
            {
                Tracked = tracked,
                Marker = marker
            };
            _markers.Add(pair);
        }

        /// <summary>
        /// Updates marker positions according to current map view
        /// </summary>
        private void Update()
        {
            foreach (var marker in _markers)
            {
                marker.Marker.anchoredPosition = WorldToMapCoordinates(marker.Tracked.position);
                var cameraDirection = Vector3.ProjectOnPlane(marker.Tracked.forward, Vector3.up).normalized;
                var heading = Vector3.SignedAngle(Vector3.forward, cameraDirection, Vector3.up);
                marker.Marker.rotation = Quaternion.AngleAxis(heading, -Vector3.forward);
            }
        }
        
        /// <summary>
        /// Initializes map view
        /// </summary>
        private void Start()
        {
            Loader.Instance.afterLoading.AddListener(() => mapImage.texture = ComputeProxy.Instance.PreviewHeightmap(mapResolution) );
            
            InputProvider.Instance.Input.Map.MoveDelta.performed += OnMoveDeltaPerformed;
            InputProvider.Instance.Input.Map.Scale.performed += OnScaleChange;
            InputProvider.Instance.Input.Map.Drag.performed += _ => _isDragging = _isMouseOver;
            InputProvider.Instance.Input.Map.Drag.canceled += _ => _isDragging = false;
        }

        public void OnShow()
        {
            InputProvider.Instance.Input.Map.Enable();
        }

        public void OnHide()
        {
            InputProvider.Instance.Input.Map.Disable();
        }

        /// <summary>
        /// Moves map by said offset
        /// </summary>
        /// <param name="delta">Map offset in world coordinates</param>
        private void MoveMap(Vector2 delta)
        {
            var moved = mapTransform.anchoredPosition + delta;
            var boundary = 500 * _scale;
            
            moved = new Vector2(
                Mathf.Clamp(moved.x, -boundary, boundary),
                Mathf.Clamp(moved.y, -boundary, boundary)
                );

            mapTransform.anchoredPosition = moved;
        }
        
        /// <summary>
        /// Converts world position to map cooordinates
        /// </summary>
        /// <param name="worldPosition">World position to be converted</param>
        /// <returns>World position in map coordinates</returns>
        public Vector2 WorldToMapCoordinates(Vector3 worldPosition)
        {
            var flatPosition = new Vector2(worldPosition.x, worldPosition.z);
            var normalized = flatPosition / TerrainManager.Instance.terrainSettings.size * 2;
            var scaled = normalized * 500 * _scale;
            return mapTransform.anchoredPosition + scaled;
        }
        
        /// <summary>
        /// Mouse movement input handler
        /// </summary>
        private void OnMoveDeltaPerformed(InputAction.CallbackContext context)
        {
            // For some reason input system started to return wrong delta values
            // Fine, ill do it myself

            var current = Mouse.current.position.ReadValue(); 
            var delta = current - _lastMousePosition;
            _lastMousePosition = current;
        
            if (_isDragging && _isMouseOver)
                MoveMap(delta / mapCanvas.scaleFactor);
        
        }
        
        /// <summary>
        /// Map scale input handler
        /// </summary>
        private void OnScaleChange(InputAction.CallbackContext context)
        {
            var delta = context.ReadValue<Vector2>();

            var lastScale = _scale;
            _scale = Mathf.Clamp(_scale + delta.y * 0.25f, minScale, maxScale);
            mapTransform.localScale = new Vector3(_scale, _scale, 1);

            scaleSlider.SetValueWithoutNotify(_scale);
            
            var mousePos = Mouse.current.position.ReadValue() / mapCanvas.scaleFactor - new Vector2(1920/2f, 1080/2f);
            var mapCoordinates = mousePos - mapTransform.anchoredPosition;
            
            var offset = mapCoordinates - mapCoordinates * ( _scale / lastScale);
            MoveMap(offset);
        }

        /// <summary>
        /// Sets map scale
        /// </summary>
        /// <param name="scale"></param>
        public void SetScale(float scale)
        {
            var mapCoordinates = -mapTransform.anchoredPosition;
            var offset = mapCoordinates - mapCoordinates * ( scale / _scale);
            _scale = scale;
            mapTransform.localScale = new Vector3(_scale, _scale, 1);
            MoveMap(offset);
        }
    }
}
