using Game.Mission;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Class for visualization and picking of missions
    /// </summary>
    public class MissionListing : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        
        private Mission _mission;

        public void SetMission(Mission mission)
        {
            _mission = mission;
            titleText.text = mission.Description;
        }

        public void Preview()
        {
            MissionView.Instance.ShowPreview(_mission);
        }
    }
}
