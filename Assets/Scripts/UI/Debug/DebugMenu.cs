using System;
using System.Collections.Generic;
using System.Linq;
using Terrain.Demo;
using TMPro;
using UnityEngine;

namespace UI.Debug
{
    /// <summary>
    /// Shows helpful information in debug window
    /// </summary>
    public class DebugMenu : MonoBehaviour
    {
        /// <summary>
        /// FPS samples
        /// </summary>
        private int _samples = 30;
        
        /// <summary>
        /// List of FPS samples
        /// </summary>
        private List<float> _deltas = new List<float>();
        
        /// <summary>
        /// Text object for FPS
        /// </summary>
        [SerializeField] private TMP_Text fpsDisplay;
        
        /// <summary>
        /// Demo settings instance
        /// </summary>
        [Header("Foliage")]
        [SerializeField] private Terrain.Demo.Settings _settings;
        
        /// <summary>
        /// Text displaying current foliage render distance
        /// </summary>
        public TMP_Text RenderDistanceText;
        
        /// <summary>
        /// Text displaying currect foliage render state
        /// </summary>
        public TMP_Text RenderFlagText;
        
        /// <summary>
        /// Camera rigidbody reference
        /// </summary>
        [Header("Player")]
        [SerializeField] private Rigidbody _cameraBody;
        
        /// <summary>
        /// Text displaying camera position
        /// </summary>
        public TMP_Text CameraPositionText;
        
        /// <summary>
        /// Text displaying camera velocity
        /// </summary>
        public TMP_Text CameraVelocityText;
        
        /// <summary>
        /// Initial buffer filling
        /// </summary>
        private void Start()
        {
            // Initialize sample buffer
            for (int i = 0; i < _samples; i++)
                _deltas.Add(0);
        }

        /// <summary>
        /// Sample FPS and update all texts
        /// </summary>
        void Update()
        {
            _deltas.RemoveAt(0);
            _deltas.Add(Time.unscaledDeltaTime);
            
            float fps = 1f / _deltas.Average();
            fpsDisplay.SetText($"FPS: {(int)fps}");
            
            RenderFlagText.text = $"Foliage rendering: {(_settings.RenderFoliage ? "enabled" : "disabled" )}";
            RenderDistanceText.text = $"Foliage distance: {_settings.FoliageRenderDistance}m";            

            CameraPositionText.text = $"Position: x={(int)_cameraBody.position.x} y= {(int)_cameraBody.position.y} z= {(int)_cameraBody.position.z}";
            CameraVelocityText.text = $"Velocity: {(int)_cameraBody.linearVelocity.magnitude} mps ";
        }
    }
}
