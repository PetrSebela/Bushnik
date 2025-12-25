using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD
{
    /// <summary>
    /// Class for managing bar ticks
    /// </summary>
    public class BarTick : MonoBehaviour
    {
        /// <summary>
        /// List of all sub sprites
        /// </summary>
        public RectTransform[] Partial;
        
        /// <summary>
        /// All images comprising the bar 
        /// </summary>
        public Image[] Bars; 

        /// <summary>
        /// Sets width of bar tick
        /// </summary>
        /// <param name="width">Width of the bar tick</param>
        public void SetWidth(float width)
        {
            foreach (var partial in Partial)
                partial.sizeDelta = new Vector2(width, 1);
        }

        /// <summary>
        /// Sets color of said bar ticks
        /// </summary>
        /// <param name="color">Color of the bar </param>
        public void SetColor(Color color)
        {
            foreach (var bar in Bars)
                bar.color = color;
        }
    }
}