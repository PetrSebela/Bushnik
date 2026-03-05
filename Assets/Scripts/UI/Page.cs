using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace UI
{
    /// <summary>
    /// Component representing single UI page
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class Page : MonoBehaviour
    {
        /// <summary>
        /// Page preceding current page
        /// </summary>
        public Page precedingPage;
        
        /// <summary>
        /// Called upon showing page
        /// </summary>
        public UnityEvent onShow;
        
        /// <summary>
        /// Called upon hiding page
        /// </summary>
        public UnityEvent onHide;
        
        /// <summary>
        /// Arbitrary action map for active page
        /// </summary>
        ///
        public PageActions[] actions;

        /// <summary>
        /// If current page is shown
        /// </summary>
        private bool _shown = false;
        
        /// <summary>
        /// Canvas group for current page
        /// </summary>
        private CanvasGroup _canvasGroup;

        public void Init()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            gameObject.SetActive(true);
            Hide(true);
        }
        
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            
            foreach (var action in actions)
                action.trigger.action.performed += _ =>
                {
                    if (!_shown)
                        return;
                    action.action.Invoke();
                };
        }
        
        /// <summary>
        /// Navigates backwards
        /// </summary>
        public void Back()
        {
            if (precedingPage == null)
                return;
            
            PageManager.Instance.Show(precedingPage);
        }

        public void Show()
        {
            PageManager.Instance.Show(this);
        }
        
        /// <summary>
        /// Shows page
        /// </summary>
        /// <param name="instant">If transition should be instant</param>
        public void Enable(bool instant=false)
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
        
        /// <summary>
        /// Hides page
        /// </summary>
        /// <param name="instant">If transition should be instant</param>
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

    /// <summary>
    /// Struct for arbitrary action map
    /// </summary>
    [System.Serializable]
    public struct PageActions
    {
        public InputActionReference trigger;
        public UnityEvent action;
    }
}
