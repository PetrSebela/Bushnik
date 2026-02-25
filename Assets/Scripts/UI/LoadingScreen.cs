using System;
using TMPro;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class LoadingScreen : MonoBehaviour
    {
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
