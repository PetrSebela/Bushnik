using System.Collections.Generic;
using Game.World;
using Terrain;
using Terrain.Interests;
using UI;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace Game.Mission
{
    /// <summary>
    /// Class responsible for management of missions
    /// </summary>
    public class MissionManager : Singleton<MissionManager>
    {
        /// <summary>
        /// Currently active mission
        /// </summary>
        private Mission _activeMission;
        
        /// <summary>
        /// Actions performed when current mission is changed
        /// </summary>
        public UnityEvent<Mission> onMissionChanged = new();


        private void Start()
        {
            onMissionChanged.Invoke(null);
        }
        
        /// <summary>
        /// Generates available mission for current location
        /// </summary>
        /// <returns>List of available missions</returns>
        public List<Mission> GetAvailableMissions()
        {
            List<Mission> missions = new();

            if (GameManager.Instance.landedAt == null)
                return missions;

            foreach (var poi in TerrainFeatureManager.Instance.Interests)
            {
                if(poi == GameManager.Instance.landedAt)
                    continue;

                foreach (var missionTemplate in poi.Missions)
                {
                    var mission = new Mission(missionTemplate.description, GameManager.Instance.landedAt, poi);
                    missions.Add(mission);
                }
            }
            
            return missions;
        }

        /// <summary>
        /// Completes current mission
        /// TODO: add check if the completion is valid
        /// </summary>
        public void CompleteMission()
        {
            _activeMission = null;
            onMissionChanged.Invoke(_activeMission);
            Announcer.Instance.Announce("Mission completed");
        }
        
        /// <summary>
        /// Starts provided message
        /// </summary>
        /// <param name="mission">Mission to be started</param>
        public void StartMission(Mission mission)
        {
            _activeMission = mission;
            onMissionChanged.Invoke(_activeMission);
            Announcer.Instance.Announce($"Deliver cargo to {_activeMission.Destination.name}");
        }
    }
}
