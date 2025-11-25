using System;
using Terrain.Foliage;
using TMPro;
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
        /// Actual foliage render ditance
        /// </summary>
        public float FoliageRenderDistance = 1000;
        
        /// <summary>
        /// Increments for foliage render distance
        /// </summary>
        public float RenderDistanceIncrement = 100;
        
        void Start()
        {
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
            FoliageRenderDistance += delta;
            FoliageRenderDistance = Math.Max(0, FoliageRenderDistance);
            FoliageManager.Instance.RenderDistance = FoliageRenderDistance;
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
