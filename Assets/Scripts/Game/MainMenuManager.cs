using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup mainMenu;
        [SerializeField] private CanvasGroup gamePicker;
        [SerializeField] private CanvasGroup optionsMenu;
        [SerializeField] private float transitionTime = 0.125f;
        
        private CanvasGroup[] _groups;
        
        void Start()
        {
            _groups = new [] { mainMenu, gamePicker, optionsMenu};
            foreach(var group in _groups)
                group.gameObject.SetActive(true);
            ShowMainMenu();
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

        public void ShowGamePicker()
        {
            Show(gamePicker);
        }

        public void ShowMainMenu()
        {
            Show(mainMenu);
        }

        public void ShowOptions()
        {
            Show(optionsMenu);
        }
        
        public void LoadGame()
        {
            SceneManager.LoadScene("World");   
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
