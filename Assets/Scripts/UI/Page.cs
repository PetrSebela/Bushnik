using Game.World;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Page : MonoBehaviour
    {
        public Page precedingPage;
        public UnityEvent onShow;
        public UnityEvent onHide;
        private CanvasGroup _canvasGroup;
        
        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Back()
        {
            if (precedingPage == null)
                return;
            
            PageManager.Instance.Show(precedingPage);
        }
        
        public void Show()
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
            LeanTween.cancel(gameObject);
            LeanTween.alphaCanvas(_canvasGroup, 1, 0.1f).setIgnoreTimeScale(true);
            onShow?.Invoke();
        }

        public void ForceShow()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }

        public void ForceHide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }
        
        public void Hide()
        {
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
            LeanTween.cancel(gameObject);
            LeanTween.alphaCanvas(_canvasGroup, 0, 0.1f).setIgnoreTimeScale(true);
            onHide?.Invoke();
        }
    }
}
