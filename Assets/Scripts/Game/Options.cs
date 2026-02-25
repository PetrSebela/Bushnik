using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// Class responsible for management of player preferences
    /// </summary>
    public class Options : MonoBehaviour
    {
        /// <summary>
        /// Dropdown containing all possible resolutions
        /// </summary>
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        
        /// <summary>
        /// Dropdown containing all possible fps lock values
        /// </summary>
        [SerializeField] private TMP_Dropdown frameRateDropdown;
        
        /// <summary>
        /// All possible resolutions
        /// </summary>
        private readonly List<Resolution> _resolutions = new();
        
        /// <summary>
        /// All possible frame rates
        /// </summary>
        private readonly List<int> _frameRate = new(){-1, 60, 144};
        
        /// <summary>
        /// Names of all possible frame rates
        /// </summary>
        private readonly List<string> _frameRateOptions = new(){"Unlimited", "60", "144"};
        
        private void TryAddResolution(Resolution resolution)
        {
            if (_resolutions.Any(existing => existing.width == resolution.width && existing.height == resolution.height))
                return;

            _resolutions.Add(resolution);
        }
        
        private List<string> GetResolutionOptions()
        {
            List<string> options = new();
            
            foreach (var resolution in _resolutions)
                options.Add(resolution.width + " x " + resolution.height);
            
            return options;
        }

        int GetCurrentResolutionIndex()
        {
            for (int i = 0; i < _resolutions.Count; i++)
            {
                if (_resolutions[i].width == Screen.currentResolution.width &&
                    _resolutions[i].height == Screen.currentResolution.height)
                    return i;
            }
            
            return -1;
        }

        void Start()
        {
            _resolutions.Clear();
            foreach (var resolution in Screen.resolutions)
                TryAddResolution(resolution);
            
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(GetResolutionOptions());
            resolutionDropdown.SetValueWithoutNotify(GetCurrentResolutionIndex());
            
            frameRateDropdown.ClearOptions();
            frameRateDropdown.AddOptions(_frameRateOptions);
        }
        
        public void SetResolution(int resolutionIndex)
        {
            var resolution = _resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void SetFullscreen(bool fullscreen)
        {
            Screen.fullScreen = fullscreen;
        }
        
        public void SetRefreshRate(int targetRefreshRate)
        {
            var refreshRate = _frameRate[targetRefreshRate];
            Application.targetFrameRate = refreshRate;
        }
    }
}
