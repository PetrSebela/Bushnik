using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Options
{
    public class FramerateDropdown : MonoBehaviour
    {
        /// <summary>
        /// Dropdown containing all possible fps lock values
        /// </summary>
        [SerializeField] private TMP_Dropdown frameRateDropdown;
        
        /// <summary>
        /// All possible frame rates
        /// </summary>
        private readonly List<int> _frameRate = new(){-1, 60, 144};
        
        /// <summary>
        /// Names of all possible frame rates
        /// </summary>
        private readonly List<string> _frameRateOptions = new(){"Unlimited", "60", "144"};
        
        /// <summary>
        /// Key under which the option values is stored in player prefs 
        /// </summary>
        private const string PrefKey = "Framerate";
        
        private void Awake()
        {
            frameRateDropdown.ClearOptions();
            frameRateDropdown.AddOptions(_frameRateOptions);
            
            var currentFramerate = PlayerPrefs.GetInt(PrefKey, 0);
            frameRateDropdown.SetValueWithoutNotify(currentFramerate);
            frameRateDropdown.onValueChanged.AddListener(SetRefreshRate);
        }
        
        private void SetRefreshRate(int targetRefreshRate)
        {
            var refreshRate = _frameRate[targetRefreshRate];
            Application.targetFrameRate = refreshRate;
            PlayerPrefs.SetInt(PrefKey, targetRefreshRate);
        }
    }
}
