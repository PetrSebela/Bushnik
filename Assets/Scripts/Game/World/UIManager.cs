using System;
using UI;
using UnityEngine;

namespace Game.World
{
    /// <summary>
    /// Class responsible for management of individual UI screens during gameplay
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup hud;
        [SerializeField] private CanvasGroup pauseMenu;
        [SerializeField] private CanvasGroup optionsMenu;
        [SerializeField] private CanvasGroup loadingScreen;
        [SerializeField] private float transitionTime = 0.125f;
        
        private CanvasGroup[] _groups;
        
        private static UIManager _instance;
        
        public static UIManager Instance => _instance;

        private UIState _state = UIState.Loading;
        public UIState State => _state;
        
        public enum UIState
        {
            Loading,
            Game,
            Pause,
            Options
        }
        
        public void BeginLoading()
        {
            _state = UIState.Loading;
            Show(loadingScreen);
        }

        public void FinishLoading()
        {
            _state = UIState.Game;
            Show(hud);
        }
        
        void Awake()
        {
            _instance = this;
            if (loadingScreen == null)
                _groups = new [] { hud, pauseMenu, optionsMenu};
            else
                _groups = new [] { hud, pauseMenu, optionsMenu, loadingScreen};
        }
        
        
        void Show(CanvasGroup active)
        {
            foreach (var canvas in _groups )
            {
                if(canvas == active)
                    continue;
                
                LeanTween.alphaCanvas(canvas, 0, transitionTime).setIgnoreTimeScale(true);
                canvas.interactable = false;
                canvas.blocksRaycasts = false;
            }
            
            LeanTween.alphaCanvas(active, 1, transitionTime).setIgnoreTimeScale(true);
            active.interactable = true;
            active.blocksRaycasts = true;
        }

        public void Back()
        {
            switch (_state)
            {
                case UIState.Game:
                    ShowPauseMenu();
                    break;
                
                case UIState.Pause:
                    ShowHUD();
                    break;
                
                case UIState.Options:
                    ShowPauseMenu();
                    break;
            }
        }

        public void ShowPauseMenu()
        {
            _state = UIState.Pause;
            Show(pauseMenu);
        }

        public void ShowOptionsMenu()
        {
            _state = UIState.Options;
            Show(optionsMenu);
        }

        public void ShowHUD()
        {
            _state = UIState.Game;
            Show(hud);
        }
    }
}
