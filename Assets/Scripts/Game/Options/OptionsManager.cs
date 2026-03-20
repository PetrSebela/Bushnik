using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Utility;

namespace Game.Options
{
    /// <summary>
    /// Class responsible for management of player preferences
    /// </summary>
    public class OptionsManager : Singleton<OptionsManager>
    {
        public ResolutionDropdown resolution;
        public FramerateDropdown framerate;
        public FoliageDistance foliageDistance;
    }
}
