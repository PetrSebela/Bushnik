using System;
using Terrain;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using User;

namespace UI.Map
{
    public class MapView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Canvas mapCanvas;
        [SerializeField] private RawImage mapImage;
        [SerializeField] private int mapResolution;
        [SerializeField] private RectTransform mapTransform;
        [SerializeField] private float minScale;
        [SerializeField] private float maxScale;
        [SerializeField] private Slider scaleSlider;
        
        private bool _isDragging = false;
        private float _scale = 1;
        
        private static MapView _instance;
        public static MapView Instance => _instance;

        private bool _isMouseOver = false;
        
        void Awake()
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

        void Start()
        {
            Loader.Instance.AfterLoading.AddListener(UpdateMap);
            
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
        
        public Vector2 WorldToMapCoordinates(Vector3 worldPosition)
        {
            var flatPosition = new Vector2(worldPosition.x, worldPosition.z);
            var normalized = flatPosition / TerrainManager.Instance.terrainSettings.size * 2;
            var scaled = normalized * 500 * _scale;
            return mapTransform.anchoredPosition + scaled;
        }
        
        void UpdateMap()
        {
            mapImage.texture = ComputeProxy.Instance.PreviewHeightmap(mapResolution);
            UnityEngine.Debug.Log("Map updated");
        }

        void OnMoveDeltaPerformed(InputAction.CallbackContext context)
        {
            var delta = context.ReadValue<Vector2>();
            if(_isDragging && _isMouseOver)
                MoveMap(delta / mapCanvas.scaleFactor);
        }
        
        void OnScaleChange(InputAction.CallbackContext context)
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
