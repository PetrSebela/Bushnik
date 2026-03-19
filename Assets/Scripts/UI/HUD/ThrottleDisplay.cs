using Aircraft.Controller;
using TMPro;
using UnityEngine;

namespace UI.HUD
{
    public class ThrottleDisplay : MonoBehaviour
    {
        /// <summary>
        /// How much space does the throttle indicator take up
        /// </summary>
        [SerializeField] private float throttleBarPercentage;
        
        /// <summary>
        /// How much space does the brake indicator take up
        /// </summary>
        [SerializeField] private float brakeBarPercentage;
        
        /// <summary>
        /// Throttle indicator bar
        /// </summary>
        [SerializeField] private RectTransform throttleBar;
        
        /// <summary>
        /// Brake indicator bar
        /// </summary>
        [SerializeField] private RectTransform brakeBar;
        
        /// <summary>
        /// Text displaying the current throttle/brake value
        /// </summary>
        [SerializeField] private TMP_Text readoutText;

        private RectTransform _container;

        void Awake()
        {
            _container = GetComponent<RectTransform>();
        }

        public void SetValue(float throttle, float brake)
        {
            throttleBar.sizeDelta = new Vector2(throttleBar.sizeDelta.x, throttle * throttleBarPercentage * _container.sizeDelta.y );
            brakeBar.sizeDelta = new Vector2(brakeBar.sizeDelta.x, brake * brakeBarPercentage * _container.sizeDelta.y );
            
            readoutText.text = ((int)(throttle * 100)).ToString();
            if(brake != 0)
                readoutText.text = "BRK";
        }
    }
}
