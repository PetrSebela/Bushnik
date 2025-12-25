using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI.HUD
{
    /// <summary>
    /// Class responsible for managing HUD compass
    /// </summary>
    public class Compass : MonoBehaviour
    {
        
        /// <summary>
        /// Degrees that will be displayed on the compass ( True north is 0 deg )
        /// </summary>
        public float degrees = 0;
        
        /// <summary>
        /// Named tick spaced 90 degrees apart
        /// </summary>
        [SerializeField] private GameObject majorTick;
        
        /// <summary>
        /// Tick placed every 10 degrees
        /// </summary>
        [SerializeField] private GameObject minorTick;
        
        /// <summary>
        /// Parent of generated ticks
        /// </summary>
        [SerializeField] private RectTransform compassContainer;

        /// <summary>
        /// Visible range
        /// </summary>
        [SerializeField] private float visibleRange;
        
        /// <summary>
        /// Label with accurate angle display
        /// </summary>
        [SerializeField] private TMP_Text readout;
        
        /// <summary>
        /// Maps angle to specific tick
        /// </summary>
        private readonly Dictionary<float, GameObject> _ticks = new();

        void Start()
        {
            for (int angle = 0; angle < 360; angle += 5)
            {
                if (angle % 45 == 0)
                {
                    var majorTickInstance = Instantiate(majorTick, compassContainer);
                    var label = majorTickInstance.GetComponentInChildren<TextMeshProUGUI>();
                    
                    label.text = angle switch
                    {
                        0 => "N",
                        45 => "NE",
                        90 => "E",
                        135 => "SE",
                        180 => "S",
                        225 => "SW",
                        270 => "W",
                        315 => "NW",
                        _ => ""
                    };
                    _ticks.Add(angle, majorTickInstance);
                }
                else if (angle % 5 == 0)
                {
                    GameObject minorTickInstance = Instantiate(minorTick, compassContainer);
                    _ticks.Add(angle, minorTickInstance);
                }
            }
        }
        
        private void Update()
        {
            foreach (var pair in _ticks)
            {
                var tick = pair.Value;
                var angle = pair.Key;
                angle = ((angle - degrees) % 360 + 360) % 360;
                
                if(angle > 180)
                    angle -= 360;
                
                // Disable hidden ticks
                bool enable = Mathf.Abs(angle) < visibleRange;
                tick.SetActive(enable);
                if(!enable)
                    continue;
                    
                var position = (angle / visibleRange) * (compassContainer.rect.width / 2);
                tick.transform.localPosition = new Vector3(position, 0, 0);
            }

            var mapped = ((int)degrees % 360 + 360) % 360;
            readout.text = $"{mapped}";
        }
    }
}
