using Terrain.Interests;
using UnityEngine;

namespace Game.Mission
{
    public class Mission
    {
        public string Description;
        public PointOfInterest Origin;
        public PointOfInterest Destination;

        public Mission(string description, PointOfInterest origin, PointOfInterest destination)
        {
            Description = description;
            Origin = origin;
            Destination = destination;
        }
    }
}
