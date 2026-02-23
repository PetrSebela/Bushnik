using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] public CanvasGroup hudGroup;

        private static UIManager _instance;
        
        public static UIManager Instance => _instance;
        
        private void Awake()
        {
            _instance = this;
        }
        
        public void HideHUD(bool instant=false)
        {
            if(instant)
                hudGroup.alpha = 0;
            else
                LeanTween.alphaCanvas(hudGroup, 0, 1).setIgnoreTimeScale(true);
        }

        public void ShowHUD(bool instant=false)
        {
            if(instant)
                hudGroup.alpha = 1;
            else
                LeanTween.alphaCanvas(hudGroup, 1, 1).setIgnoreTimeScale(true);
        }
    }
}
