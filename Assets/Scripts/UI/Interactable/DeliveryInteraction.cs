using Game.Mission;
using Terrain.Interests;

namespace UI.Interactable
{
    /// <summary>
    /// Interaction for compleating deliveries
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
        }
        
        public override void Interact()
        {
            MissionManager.Instance.CompleteMission();
        }
        
        /// <summary>
        /// Dynamically disables delivery point
        /// </summary>
        public void Update()
        {
            if(MissionManager.Instance.ActiveMission == null)
                Disable();
            else if (MissionManager.Instance.ActiveMission.Destination == _onAirport)
                Enable();
            else
                Disable();
        }
    }
}
