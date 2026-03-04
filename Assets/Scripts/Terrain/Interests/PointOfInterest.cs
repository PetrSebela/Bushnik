using System;
using System.Collections.Generic;
using Game.World;
using UI;
using UnityEngine;

namespace Terrain.Interests
{
    public class PointOfInterest : MonoBehaviour
    {
        public readonly List<TerrainAffectorData> TerrainAffectors = new();
        public bool landed;
        
        private void OnTriggerStay(Collider other)
        {
            if (landed)
                return;

            if (GameManager.Instance.Player.linearVelocity.magnitude > 5f)
                return;
            
            landed = true;
            Announcer.Instance.Announce($"Landed at {name}");
        }

        private void OnTriggerExit(Collider other)
        {
            if(landed)
                Announcer.Instance.Announce($"Departed from {name}");
            
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
