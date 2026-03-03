using System;
using UnityEngine;
using UnityEngine.InputSystem;
using User;

namespace UI
{
    /// <summary>
    /// Class managing paces in scene
    /// </summary>
    public class PageManager : MonoBehaviour
    {
        /// <summary>
        /// What page should be active upon scene load
        /// </summary>
        public Page defaultPage;

        /// <summary>
        /// Currently active page
        /// </summary>
        private Page _currentPage;

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static PageManager _instance;
        
        /// <summary>
        /// Instance getter
        /// </summary>
        public static PageManager Instance => _instance;
        
        public void Awake()
        {
            _instance = this;
        }
        
        /// <summary>
        /// Sets all pages to consistent state
        /// </summary>
        public void Start()
        {
            foreach (var top in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            {
                foreach (var page in top.GetComponentsInChildren<Page>(true))
                {
                    page.gameObject.SetActive(true);
                    page.Hide(true);
                }
            }

            _currentPage = defaultPage;
            defaultPage.Show(true);
            
            InputProvider.Instance.Input.UI.Back.performed += Back;
        }

        /// <summary>
        /// Shows page
        /// </summary>
        /// <param name="page">Page to be shown</param>
        public void Show(Page page)
        {
            _currentPage.Hide();
            _currentPage = page;
            page.Show();
        }

        /// <summary>
        /// Handler for back action
        /// </summary>
        private void Back(InputAction.CallbackContext context)
        {
            if(_currentPage.precedingPage == null)
                return;
            
            Show(_currentPage.precedingPage);
        }
    }
}
