using Game.World;
using Terrain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Game.Options
{
    public class DemoConfig : MonoBehaviour
    {
        [SerializeField] private TerrainSettings terrainSettings;
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private GameObject[] aircraftPrefabs;
        private int _selectedAircraftIndex = 0;

        [Header("UI")]
        [SerializeField] private Slider worldSizeSlider;
        [SerializeField] private TMP_Text worldSizePreview;
        [SerializeField] private Slider heightSlider;
        [SerializeField] private TMP_Text heightPreview;
        [SerializeField] private TMP_Dropdown aircraftDropdown;
        [SerializeField] private TMP_InputField seedInputField;
        
        private void Awake()
        {
            worldSizeSlider.value = terrainSettings.size / 10000;
            worldSizePreview.text = $"{terrainSettings.size / 1000} Km";
            worldSizeSlider.onValueChanged.AddListener(SetWorldSize);
            
            heightSlider.value = terrainSettings.height / 100;
            heightPreview.text = $"{terrainSettings.height} m";
            heightSlider.onValueChanged.AddListener(SetTerrainHeight);
            
            aircraftDropdown.value = _selectedAircraftIndex;
            gameConfig.defaultAircraft = aircraftPrefabs[_selectedAircraftIndex];
            aircraftDropdown.onValueChanged.AddListener(SelectAircraft);
            
            seedInputField.text = "";
            seedInputField.onValueChanged.AddListener(SetSeed);
        }
        
        private void SetWorldSize(float size)
        {
            terrainSettings.size = size * 10000;
            worldSizePreview.text = $"{size * 10000 / 1000} Km";
        }

        private void SetTerrainHeight(float height)
        {
            terrainSettings.height = height * 100;
            heightPreview.text = $"{height * 100} m";
        }
        
        private void SetSeed(string seed)
        {
            terrainSettings.seed = seed;
        }

        private void SelectAircraft(int index)
        {
            _selectedAircraftIndex = index;
            gameConfig.defaultAircraft = aircraftPrefabs[_selectedAircraftIndex];
        }
    }
}
