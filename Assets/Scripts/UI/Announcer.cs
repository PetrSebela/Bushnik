using System;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Class for announcing messages to the player
    /// </summary>
    public class Announcer : MonoBehaviour
    {
        /// <summary>
        /// Text display
        /// </summary>
        [SerializeField] private TMP_Text announcerText;
        
        /// <summary>
        /// Canvas group for managing visibility
        /// </summary>
        [SerializeField] private CanvasGroup canvasGroup;
        
        /// <summary>
        /// Singleton instance
        /// </summary>
        private static Announcer _instance;
        
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static Announcer Instance => _instance;

        private void Awake()
        {
            _instance = this;
        }

        /// <summary>
        /// Displays message on the hud
        /// </summary>
        /// <param name="announcement"></param>
        public void Announce(string announcement)
        {
            announcerText.text = announcement;
            var tween = LeanTween.alphaCanvas(canvasGroup, 1, 0.25f).setIgnoreTimeScale(true);
            tween.setOnComplete(_ => LeanTween.delayedCall(5, FadeOut).setIgnoreTimeScale(true));
        }

        private void FadeOut()
        {
            LeanTween.alphaCanvas(canvasGroup, 0, 0.25f).setIgnoreTimeScale(true);
        }
    }
}
