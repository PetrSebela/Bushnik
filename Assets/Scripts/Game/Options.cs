using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class Options : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown frameRateDropdown;
        
        private readonly List<Resolution> _resolutions = new();
        private readonly List<int> _frameRate = new(){-1};
        private readonly List<string> _frameRateOptions = new(){"Unlimited"};
        
        private void TryAddResolution(Resolution resolution)
        {
            if (_resolutions.Any(existing => existing.width == resolution.width && existing.height == resolution.height))
                return;

            _resolutions.Add(resolution);
        }

        private void TryAddFrameRate(int targetFrameRate)
        {
            if (_frameRate.Any(existing => existing == targetFrameRate))
                return;
            
            _frameRate.Add(targetFrameRate);
            _frameRateOptions.Add(targetFrameRate.ToString());
        }

        private List<string> GetResolutionOptions()
        {
            List<string> options = new();
            
            foreach (var resolution in _resolutions)
                options.Add(resolution.width + " x " + resolution.height);
            
            return options;
        }

        void Start()
        {
            _resolutions.Clear();
            foreach (var resolution in Screen.resolutions)
            {
                TryAddResolution(resolution);
                TryAddFrameRate((int)resolution.refreshRateRatio.value);
            }
            
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(GetResolutionOptions());
            
            frameRateDropdown.ClearOptions();
            frameRateDropdown.AddOptions(_frameRateOptions);
        }

        void LoadConfig()
        {
            
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
