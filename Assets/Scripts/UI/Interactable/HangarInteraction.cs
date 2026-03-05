using System;
using Game.Mission;
using Terrain.Interests;
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

        /// <summary>
        /// Dynamically set status of delivery point
        /// </summary>
        public void Update()
        {
            if (MissionManager.Instance.ActiveMission == null)
                Enable();
            else
                Disable();
        }
    }
}
