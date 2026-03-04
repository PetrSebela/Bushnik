using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class Announcer : MonoBehaviour
    {
        [SerializeField] private TMP_Text announcerText;
        [SerializeField] private CanvasGroup canvasGroup;
        
        private static Announcer _instance;
        public static Announcer Instance => _instance;

        private void Awake()
        {
            _instance = this;
        }

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
