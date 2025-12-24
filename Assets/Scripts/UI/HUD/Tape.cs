using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace UI.HUD
{
    /// <summary>
    /// Class for controling HUD tape element
    /// </summary>
    public class Tape : MonoBehaviour
    {
        /// <summary>
        /// Value displayed on the tape
        /// </summary>
        public float value = 0;

        /// <summary>
        /// If tape value should be displayed as kilo
        /// </summary>
        [SerializeField] private bool showKilo = true;
        
        /// <summary>
        /// Number of indents between major ticks
        /// </summary>
        [SerializeField] private int majorTickSpacing = 10;
        
        /// <summary>
        /// Major tick prefab - should have label for displaying  value
        /// </summary>
        [SerializeField] private GameObject majorTick;
        
        /// <summary>
        /// Minor tick prefab
        /// </summary>
        [SerializeField] private GameObject minorTick;
        
        /// <summary>
        /// Minimum tape value
        /// </summary>
        public float minValue = 0;
        
        /// <summary>
        /// Maximum tape value
        /// </summary>
        public float maxValue = 1000;
        
        /// <summary>
        /// Tick spacing in physical space
        /// </summary>
        [SerializeField] private float tickSpacing;
        
        /// <summary>
        /// Tick spacing in value space
        /// </summary>
        [SerializeField] private float tickNumericalSpacing;
        
        /// <summary>
        /// Moved tape parent object
        /// </summary>
        [SerializeField] private RectTransform tapeParent;
        
        /// <summary>
        /// Label for displaying accurate value
        /// </summary>
        [SerializeField] private TMP_Text valueDisplay;
        
        /// <summary>
        /// Collection of all major tick labels
        /// </summary>
        private List<TMP_Text> _labels = new();
        
        /// <summary>
        /// Crates tape indents
        /// </summary>
        public void Start()
        {
            float range = Mathf.Abs(maxValue - minValue);
            var tickCount = Mathf.CeilToInt(range / tickNumericalSpacing);
            var increment = range / tickCount;
            
            for (int i = 0; i <= tickCount + 1; i++)
            {
                float tickValue = minValue + increment * i; 
                GameObject tick = Instantiate(i % majorTickSpacing == 0 ? majorTick : minorTick, tapeParent);
                float offset = i * tickSpacing;
                tick.transform.localPosition = new Vector3(-tapeParent.rect.width / 2, offset, 0);
                
                var label = tick.GetComponentInChildren<TMP_Text>();
                if (label)
                {
                    _labels.Add(label);
                    if(showKilo)
                        label.text = (Mathf.Round(tickValue / 100) / 10).ToString("0.0", CultureInfo.InvariantCulture);
                    else
                        label.text = tickValue.ToString("0", CultureInfo.InvariantCulture);
                }
            }
        }
        
        /// <summary>
        /// Updates value and hides labels behind the label for accurate value 
        /// </summary>
        public void Update()
        {
            valueDisplay.text = Mathf.RoundToInt(value).ToString(CultureInfo.InvariantCulture);
            float offset = (minValue - value) / tickNumericalSpacing * tickSpacing;
            tapeParent.transform.localPosition = new Vector3(0, offset, 0);
        }
    }
}
