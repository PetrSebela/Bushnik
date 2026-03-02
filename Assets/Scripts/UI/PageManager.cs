using System;
using UnityEngine;
using UnityEngine.InputSystem;
using User;

namespace UI
{
    public class PageManager : MonoBehaviour
    {
        public Page defaultPage;

        private Page _currentPage;

        private static PageManager _instance;
        public static PageManager Instance => _instance;
        
        public void Awake()
        {
            _instance = this;
        }

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

        public void Show(Page page)
        {
            _currentPage.Hide();
            _currentPage = page;
            page.Show();
        }

        private void Back(InputAction.CallbackContext context)
        {
            if(_currentPage.precedingPage == null)
                return;
            
            Show(_currentPage.precedingPage);
        }
    }
}
