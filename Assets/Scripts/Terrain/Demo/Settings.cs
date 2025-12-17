using System;
using Terrain.Foliage;
using UnityEngine;
using UnityEngine.InputSystem;
using User;

namespace Terrain.Demo
{
    /// <summary>
    /// Class for managing settings of demo scene
    /// </summary>
    public class Settings : MonoBehaviour
    {
        /// <summary>
        /// Flag for toggling foliage rendering
        /// </summary>
        public bool RenderFoliage = true;
        
        /// <summary>
        /// Actual foliage render distance
        /// </summary>
        private float _foliageRenderDistance = 6000;
        
        /// <summary>
        /// FoliageRenderDistance accesor
        /// </summary>
        public float FoliageRenderDistance => _foliageRenderDistance;
        
        /// <summary>
        /// Increments for foliage render distance
        /// </summary>
        public float RenderDistanceIncrement = 100;
        
        void Start()
        {
            _foliageRenderDistance = FoliageManager.Instance.RenderDistance;
            RegisterInput();
        }

        /// <summary>
        /// Toggle foliage rendering
        /// </summary>
        /// <param name="context"></param>
        void OnFoliageRenderToggle(InputAction.CallbackContext context)
        {
            RenderFoliage = !RenderFoliage;
            FoliageManager.Instance.enabled = RenderFoliage;
        }

        /// <summary>
        /// Change foliage render distance
        /// </summary>
        /// <param name="context"> Input context with 1D axis</param>
        void OnFoliageRenderDistanceChanged(InputAction.CallbackContext context)
        {
            float delta =  context.ReadValue<float>() * RenderDistanceIncrement;
            _foliageRenderDistance += delta;
            _foliageRenderDistance = Math.Max(0, _foliageRenderDistance);
            
            FoliageManager.Instance.SetRenderDistance(_foliageRenderDistance);
        }
        
        /// <summary>
        /// Register input callbacks 
        /// </summary>
        public void RegisterInput()
        {
            InputProvider.Instance.Input.Demo.ChangeRenderDistance.performed += OnFoliageRenderDistanceChanged;
            InputProvider.Instance.Input.Demo.ToggleFoliage.performed += OnFoliageRenderToggle;
        }

        /// <summary>
        /// Unregister input callbacks on object disable
        /// </summary>
        public void OnDisable()
        {
            InputProvider.Instance.Input.Demo.ChangeRenderDistance.performed -= OnFoliageRenderDistanceChanged;
            InputProvider.Instance.Input.Demo.ToggleFoliage.performed -= OnFoliageRenderToggle;
        }
    }
}
