using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI.HUD
{
    /// <summary>
    /// Class for controlling HUD bar element
    /// </summary>
    public class Bar : MonoBehaviour
    {
        /// <summary>
        /// Indent marker
        /// </summary>
        [SerializeField] private GameObject indentPrefab;
        
        /// <summary>
        /// Object which will be used as tick
        /// </summary>
        [SerializeField] private GameObject tickPrefab;
        
        /// <summary>
        /// Parent under which the ticks will be placed 
        /// </summary>
        [SerializeField] private RectTransform tickParent;
        
        /// <summary>
        /// Number of ticks representing the whole value range
        /// </summary>
        [SerializeField] private int tickCount;
        
        /// <summary>
        /// Maximum value that can be displayed
        /// </summary>
        [SerializeField] private float valueRange;

        /// <summary>
        /// Text object where the value will be displayed
        /// </summary>
        [SerializeField] private TMP_Text readout;
        
        /// <summary>
        /// List of all indents
        /// </summary>
        [SerializeField] private List<BarIndent> indents;
        
        /// <summary>
        /// List of all indents tick contollers
        /// </summary>
        private readonly List<BarTick> _ticks = new ();
        
        /// <summary>
        /// Value that will be displayed
        /// </summary>
        public float value;
        
        /// <summary>
        /// Creates fixed and dynamic ticks
        /// </summary>
        void Start()
        {
            float spacing = (tickParent.rect.height - 4) / (tickCount - 1);

            indents.Sort((a, b) => a.value.CompareTo(b.value));
            
            for (int i = 0; i < tickCount; i++)
            {
                var tickObject = Instantiate(tickPrefab,  tickParent);
                tickObject.transform.localPosition = new Vector3(0, spacing * i - (tickParent.rect.height - 4) / 2, 0);
                
                var tickInstance = tickObject.GetComponent<BarTick>();
                tickInstance.SetWidth(20);
                _ticks.Add(tickInstance);
                
                float mapped = valueRange * (i / (float) tickCount);
                
                foreach (var indent in indents)
                    if (mapped > indent.value)
                        tickInstance.SetColor(indent.color);
            }
            
            foreach (var indent in indents)
            {
                var tickObject = Instantiate(indentPrefab,  tickParent);
                
                float position = tickParent.rect.height * (indent.value / valueRange) - (tickParent.rect.height - 4) / 2;
                tickObject.transform.localPosition = new Vector3(0, position, 0);
                
                var tickInstance = tickObject.GetComponent<BarTick>();
                tickInstance.SetWidth(10);
                tickInstance.SetColor(indent.color);
            }
        }

        /// <summary>
        /// Updates ticks so that they match displayed value
        /// </summary>
        void Update()
        {
            for (int i = 0; i < tickCount; i++)
            {
                var tickInstance = _ticks[i];
                float progress = i / (float) tickCount;
                tickInstance.SetWidth( progress * valueRange < value ? 20 : 0);
            }

            int percentage = Mathf.RoundToInt(value * 100);
            readout.text = $"{percentage}%";

            readout.color = Color.white;
            foreach (var indent in indents)
                if (value >= indent.value)
                    readout.color = indent.color;
        }
    }
    
    /// <summary>
    /// Struct representing fixed indents on bar
    /// </summary>
    [System.Serializable]
    public struct BarIndent
    {
        public float value;
        public Color color;
    }
}
