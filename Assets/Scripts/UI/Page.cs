using Game.World;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using User;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Page : MonoBehaviour
    {
        public Page precedingPage;
        public UnityEvent onShow;
        public UnityEvent onHide;
        
        public PageActions[] _actions;

        private bool _shown = false;
        
        private CanvasGroup _canvasGroup;
        
        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            
            foreach (var action in _actions)
                action.Trigger.action.performed += _ =>
                {
                    if (!_shown)
                        return;
                    action.Action.Invoke();
                };
        }
        
        public void Back()
        {
            if (precedingPage == null)
                return;
            
            PageManager.Instance.Show(precedingPage);
        }
        
        public void Show(bool instant=false)
        {
            _shown = true;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
            
            onShow?.Invoke();
            if(instant)
            {
                _canvasGroup.alpha = 1;
                return;
            }
            
            LeanTween.cancel(gameObject);
            LeanTween.alphaCanvas(_canvasGroup, 1, 0.1f).setIgnoreTimeScale(true);
        }
        
        public void Hide(bool instant=false)
        {
            _shown = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
            onHide?.Invoke();
            
            if(instant)
            {
                _canvasGroup.alpha = 0;
                return;
            }
            
            LeanTween.cancel(gameObject);
            LeanTween.alphaCanvas(_canvasGroup, 0, 0.1f).setIgnoreTimeScale(true);
        }
    }

    [System.Serializable]
    public struct PageActions
    {
        public InputActionReference Trigger;
        public UnityEvent Action;
    }
}
