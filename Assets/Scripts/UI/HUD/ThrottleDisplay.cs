using TMPro;
using UnityEngine;

namespace UI.HUD
{
    public class ThrottleDisplay : MonoBehaviour
    {
        [SerializeField] private float throttleBarPercentage; 
        [SerializeField] private float brakeBarPercentage;
        [SerializeField] private RectTransform throttleBar;
        [SerializeField] private RectTransform brakeBar;
        [SerializeField] private TMP_Text readoutText;
        
        private RectTransform _container;

        void Awake()
        {
            _container = GetComponent<RectTransform>();
        }

        public void SetValue(float value)
        {
            if (value == 0)
            {
                throttleBar.sizeDelta = new Vector2(throttleBar.sizeDelta.x, 0);
                brakeBar.sizeDelta = new Vector2(brakeBar.sizeDelta.x, brakeBarPercentage * _container.sizeDelta.y );
                readoutText.text = "BRK";
                return;
            }
            
            brakeBar.sizeDelta = new Vector2(brakeBar.sizeDelta.x, 0);
            throttleBar.sizeDelta = new Vector2(throttleBar.sizeDelta.x, value * throttleBarPercentage * _container.sizeDelta.y );
            readoutText.text = ((int)(value * 100)).ToString();
        }
    }
}
