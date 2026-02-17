using Terrain;
using Terrain.Data;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Rigidbody player;
        
        void Start()
        {
            player.isKinematic = true;
            Loader.Instance.AfterPregeneration += SpawnPlayer;
            Loader.Instance.AfterLoading += AfterLoad;
            Loader.Instance.Load();
        }

        void SpawnPlayer()
        {
            var runway = TerrainFeatureManager.Instance.Runways[0];
            player.position = runway.ApproachThreshold + Vector3.up;
        }

        void AfterLoad()
        {
            Debug.Log("After Load");
            player.isKinematic = false;
        }
    }
}
