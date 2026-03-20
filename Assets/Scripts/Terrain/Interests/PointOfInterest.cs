using System;
using System.Collections.Generic;
using Game.Mission;
using Game.World;
using UI;
using UnityEngine;

namespace Terrain.Interests
{
    /// <summary>
    /// Base class for points of interest
    /// </summary>
    public class PointOfInterest : MonoBehaviour
    {
        /// <summary>
        /// Required terrain modifications
        /// </summary>
        public readonly List<TerrainAffectorData> TerrainAffectors = new();
        
        /// <summary>
        /// Missions which are avialable for this point
        /// </summary>
        public readonly List<MissionTemplate> Missions = new();
        
        /// <summary>
        /// Has landed aircraft
        /// </summary>
        public bool landed;
        
        /// <summary>
        /// Checks if plane landed
        /// </summary>
        private void OnTriggerStay(Collider other)
        {
            if (landed)
                return;

            if (GameManager.Instance.AircraftRigidbody.linearVelocity.magnitude > 5f)
                return;
            
            GameManager.Instance.landedAt = this;
            landed = true;
            Announcer.Instance.Announce($"Landed at {name}");
            
        }

        /// <summary>
        /// Checks if plane departed
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            if(landed)
                Announcer.Instance.Announce($"Departed from {name}");
            GameManager.Instance.landedAt = null;
            landed = false;
        }
    }
    
    public struct TerrainAffectorData
    {
        public Vector3 From;
        public Vector3 To;
        public float Width;
    }
}
