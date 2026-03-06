using System;
using Game.Mission;
using TMPro;
using UnityEngine;

namespace UI.Missions
{
    public class FlightPlanDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text missionName;
        [SerializeField] private TMP_Text departureText;
        [SerializeField] private TMP_Text destinationText;
        [SerializeField] private TMP_Text headingText;
        [SerializeField] private TMP_Text distanceText;
        [SerializeField] private CanvasGroup activeMissionGroup;
        
        private void Awake()
        {
            MissionManager.Instance.onMissionChanged.AddListener(DisplayMission);
        }

        private void DisplayMission(Mission mission)
        {
            if (mission == null)
            {
                activeMissionGroup.alpha = 0;
                activeMissionGroup.blocksRaycasts = false;
                activeMissionGroup.interactable = false;
                return;
            }
            activeMissionGroup.alpha = 1;
            activeMissionGroup.blocksRaycasts = true;
            activeMissionGroup.interactable = true;
            
            missionName.text = mission.Description;
            departureText.text = mission.Origin.name;
            destinationText.text = mission.Destination.name;
            
            var originFlat = new Vector3(mission.Origin.transform.position.x, 0, mission.Origin.transform.position.z);
            var destinationFlat = new Vector3(mission.Destination.transform.position.x, 0, mission.Destination.transform.position.z);


            var angle = Vector3.SignedAngle(Vector3.forward, destinationFlat - originFlat, Vector3.up);
            if (angle < 0)
                angle += 360;
            
            var heading = Mathf.RoundToInt(angle);
            headingText.text = $"{heading}°";
            
            var distance = Vector3.Distance(originFlat, destinationFlat);
            var distanceKilo = Mathf.Round(distance / 100) / 10;
            distanceText.text = $"{distanceKilo} Km";
        }
    }
}
