using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Game.Options
{
    public class ResolutionDropdown : MonoBehaviour
    {
        /// <summary>
        /// Dropdown containing all possible resolutions
        /// </summary>
        [SerializeField] private TMP_Dropdown resolutionDropdown;

        /// <summary>
        /// All possible resolutions
        /// </summary>
        private readonly List<Resolution> _resolutions = new();

        /// <summary>
        /// Key under which the option values is stored in player prefs 
        /// </summary>
        private const string PrefKey = "Resolution";

        /// <summary>
        /// Merges same pixel resolution and ignores framerate
        /// </summary>
        /// <param name="resolution"></param>
        private void TryAddResolution(Resolution resolution)
        {
            if (_resolutions.Any(existing => existing.width == resolution.width && existing.height == resolution.height))
                return;

            _resolutions.Add(resolution);
        }

        /// <summary>
        /// Returns current screen resolution
        /// </summary>
        public string GetCurrentResolution()
        {
            return GetResolutionOptions()[GetCurrentResolutionIndex()];
        }
        
        /// <summary>
        /// Constructs fields for display in resolution dropdown
        /// </summary>
        /// <returns></returns>
        private List<string> GetResolutionOptions()
        {
            List<string> options = new();
            
            foreach (var resolution in _resolutions)
                options.Add(resolution.width + " x " + resolution.height);
            
            return options;
        }
        
        /// <summary>
        /// Returns current resolution index
        /// </summary>
        private int GetCurrentResolutionIndex()
        {
            for (int i = 0; i < _resolutions.Count; i++)
            {
                if (_resolutions[i].width == Screen.currentResolution.width &&
                    _resolutions[i].height == Screen.currentResolution.height)
                    return i;
            }
            
            return -1;
        }

        /// <summary>
        /// Initializes struct
        /// </summary>
        private void Awake()
        {
            foreach (var resolution in Screen.resolutions)
                TryAddResolution(resolution);
            
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(GetResolutionOptions());
            
            var currentResolutionIndex = PlayerPrefs.GetInt(PrefKey, GetCurrentResolutionIndex());
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
            resolutionDropdown.value = currentResolutionIndex;
        }
        
        /// <summary>
        /// Sets resolution associated with provided index
        /// </summary>
        private void SetResolution(int resolutionIndex)
        {
            var resolution = _resolutions[resolutionIndex];
            PlayerPrefs.GetInt(PrefKey, GetCurrentResolutionIndex());
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }
}
