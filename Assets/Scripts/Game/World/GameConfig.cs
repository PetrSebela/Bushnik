using UnityEngine;

namespace Game.World
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "GameConfig", order = 5)]
    public class GameConfig : ScriptableObject
    {
        public GameObject defaultAircraft;
    }
}
