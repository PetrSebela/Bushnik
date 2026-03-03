using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Interests
{
    public abstract class PointOfInterestDescriptor : ScriptableObject
    {
        public abstract List<PointOfInterest> GetPointOfInterest();
    }
}
