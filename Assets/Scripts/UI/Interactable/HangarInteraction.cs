using System;
using Game.Mission;
using Terrain.Interests;
using UI.Missions;
using UnityEngine;

namespace UI.Interactable
{
    /// <summary>
    /// Interaction for picking up deliveries 
    /// </summary>
    public class HangarInteraction : Interactable
    {
        public override void Interact()
        {
            MissionView.Instance.Show();
        }

        public void Start()
        {
            MissionManager.Instance.onMissionChanged.AddListener(StatusUpdate);
        }

        private void StatusUpdate(Mission mission)
        {
            if (mission == null)
                Enable();
            else
                Disable();
        }
    }
}
