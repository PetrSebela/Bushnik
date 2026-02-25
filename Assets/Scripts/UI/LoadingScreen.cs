using System;
using TMPro;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

namespace UI
{
    /// <summary>
    /// Class responsible for management of loading screen
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class LoadingScreen : MonoBehaviour
    {
        /// <summary>
        /// Progress
        /// </summary>
        [SerializeField] private Slider progressBar;
        [SerializeField] private TMP_Text progressText;
        private CanvasGroup _loadingScreen;
        
        public void Awake()
        {
            _loadingScreen = GetComponent<CanvasGroup>();
        }
        
        public void SetProgress(float value)
        {
            progressBar.value = value;
        }

        public void SetMessage(string message)
        {
            progressText.text = message;
        }
    }
}
