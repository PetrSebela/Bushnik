using Game.Mission;
using TMPro;
using UnityEngine;
using Utility;

namespace UI.Missions
{
    public class MissionView : Singleton<MissionView>
    {
        [SerializeField] private RectTransform missionParent;
        [SerializeField] private GameObject missionEntryPrefab;
        [SerializeField] private TMP_Text missionDestinationText;
        [SerializeField] private TMP_Text missionDescriptionText;
        [SerializeField] private CanvasGroup detailGroup;
        [SerializeField] private Page missionPage;
        private Mission _previewedMission = null;

        public void Show() => missionPage.Show();
        
        public void UpdateMissions()
        {
            var missions = MissionManager.Instance.GetAvailableMissions();

            while (missionParent.childCount > 0)
                DestroyImmediate(missionParent.GetChild(0).gameObject);
            
            foreach (var mission in missions)
            {
                var missionEntry = Instantiate(missionEntryPrefab, missionParent);
                missionEntry.GetComponent<MissionListing>().SetMission(mission);
            }

            _previewedMission = null;
            missionDestinationText.text = "";
            missionDescriptionText.text = "";
            detailGroup.alpha = 0;
        }

        public void ShowPreview(Mission mission)
        {
            _previewedMission = mission;
            missionDestinationText.text = $"Destination: {mission.Destination.name}";
            missionDescriptionText.text = mission.Description;
            LeanTween.alphaCanvas(detailGroup, 1, 0.125f).setIgnoreTimeScale(true);
        }

        public void AcceptPreview()
        {
            if(_previewedMission == null)
                return;
            
            UnityEngine.Debug.Log($"Accepting mission {_previewedMission.Description}");
            missionPage.Back();
            MissionManager.Instance.StartMission(_previewedMission);
        }
    }
}
