using Terrain.Foliage;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.Options
{
    public class FoliageDistance : MonoBehaviour
    {
        /// <summary>
        /// Render distance slider
        /// </summary>
        [SerializeField] private Slider distanceSlider;

        /// <summary>
        /// Label with current RD value
        /// </summary>
        [SerializeField] private TMP_Text currentFoliageDistance;
        
        /// <summary>
        /// Current render distance
        /// </summary>
        public float RenderDistance => PlayerPrefs.GetFloat(PrefKey, 6000);
        
        /// <summary>
        /// Key under which the option values is stored in player prefs 
        /// </summary>
        private const string PrefKey = "FoliageDistance";
        
        private void Awake()
        {
            distanceSlider.minValue = 0;   
            distanceSlider.maxValue = 120;
            
            currentFoliageDistance.text = ((int)RenderDistance).ToString();
            distanceSlider.SetValueWithoutNotify(RenderDistance/100);
            distanceSlider.onValueChanged.AddListener(SetRenderDistance);
        }
        
        /// <summary>
        /// Sets render distance in meters
        /// </summary>
        private void SetRenderDistance(float value)
        {
            var renderDistance = value * 100;
            PlayerPrefs.SetFloat(PrefKey, renderDistance);
            currentFoliageDistance.text = ((int)renderDistance).ToString();
            if(FoliageManager.Instance)
                FoliageManager.Instance.SetRenderDistance(renderDistance);
        }
    }
}
