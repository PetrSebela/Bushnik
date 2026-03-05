using UnityEngine;

namespace Game.Mission
{
    [CreateAssetMenu(fileName = "Mission", menuName = "Gameplay/Mission", order = 0)]
    public class MissionTemplate : ScriptableObject
    {
        public string description;
    }
}
