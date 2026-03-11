using System;
using TMPro;
using UnityEngine;
using Utility;

namespace UI
{
    /// <summary>
    /// Class for announcing messages to the player
    /// </summary>
    public class Announcer : Singleton<Announcer>
    {
        /// <summary>
        /// Text display
        /// </summary>
        [SerializeField] private TMP_Text announcerText;
        
        /// <summary>
        /// Canvas group for managing visibility
        /// </summary>
        [SerializeField] private CanvasGroup canvasGroup;

        
        private void Start()
        {
            canvasGroup.alpha = 0;
        }

        /// <summary>
        /// Displays message on the hud
        /// </summary>
        /// <param name="announcement"></param>
        public void Announce(string announcement)
        {
            announcerText.text = announcement;
            LeanTween.cancel(announcerText.gameObject);
            var tween = LeanTween.alphaCanvas(canvasGroup, 1, 0.25f).setIgnoreTimeScale(true);
            tween.setOnComplete(_ => LeanTween.delayedCall(3, FadeOut).setIgnoreTimeScale(true));
        }

        private void FadeOut()
        {
            LeanTween.alphaCanvas(canvasGroup, 0, 0.25f).setIgnoreTimeScale(true);
        }
    }
}
