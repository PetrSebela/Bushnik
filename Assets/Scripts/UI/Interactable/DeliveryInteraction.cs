using Game.Mission;
using Terrain.Interests;

namespace UI.Interactable
{
    /// <summary>
    /// Interaction for completing deliveries
    /// </summary>
    public class DeliveryInteraction : Interactable
    {
        /// <summary>
        /// On what airport is delivery point located
        /// </summary>
        private Airport _onAirport;

        public void Awake()
        {
            _onAirport = Utility.Generic.LocateObjectTowardsRoot<Airport>(transform.parent);
            MissionManager.Instance.onMissionChanged.AddListener(StatusUpdate);
        }
        
        public override void Interact()
        {
            MissionManager.Instance.CompleteMission();
        }

        private void StatusUpdate(Mission mission)
        {
            if(mission == null || mission.Destination != _onAirport)
                Disable();
            else
                Enable();
        }
    }
}
