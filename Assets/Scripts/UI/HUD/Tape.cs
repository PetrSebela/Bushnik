using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity;
using UnityEngine;

namespace UI.HUD
{
    /// <summary>
    /// Class for controlling HUD tape element
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
        /// Tick spacing in physical space
        /// </summary>
        [SerializeField] private float scale;
        
        /// <summary>
        /// Moved tape parent object
        /// </summary>
        [SerializeField] private RectTransform tapeParent;
        
        /// <summary>
        /// Label for displaying accurate value
        /// </summary>
        [SerializeField] private TMP_Text valueDisplay;
        
        /// <summary>
        /// Collection of all ticks
        /// </summary>
        private List<RectTransform> _ticks = new();
        
        /// <summary>
        /// Crates tape indents
        /// </summary>
        public void Start()
        {
            for (int i = 0; i < 20; i++)
            {
                var tick = Instantiate(majorTick, tapeParent);
                for (int j = 1; j <= 9; j++)
                {
                    var minorTick = Instantiate(this.minorTick, tick.transform);
                    minorTick.transform.localPosition = new Vector3(0, j * 10 * scale, 0);
                }
                _ticks.Add(tick.transform as RectTransform);
            }
        }

        void UpdateTapeItems(float value)
        {
            var diameter = 10;
            var origin = Mathf.Round(value / 100) - diameter;
            
            for (int i = 0; i < _ticks.Count; i++)
            {
                var tickValue = (origin + i) * (100 * scale);
                var tick = _ticks[i];       
                tick.transform.localPosition = new Vector3(tick.transform.localPosition.x, tickValue, 0);

                var label = tick.GetComponentInChildren<TMP_Text>();
                if (label)
                    label.text = ((origin + i) / 10).ToString("0.0", CultureInfo.InvariantCulture);
            }
        }
        
        /// <summary>
        /// Updates value and hides labels behind the label for accurate value 
        /// </summary>
        public void Update()
        {
            valueDisplay.text = Mathf.RoundToInt(value).ToString(CultureInfo.InvariantCulture);
            UpdateTapeItems(value);
            tapeParent.localPosition = new Vector3(0, -value * scale, 0);
        }
    }
}
