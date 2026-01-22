using UnityEngine;

namespace Terrain
{
    /// <summary>
    /// Class responsible for managing the start and run of generation pipeline
    /// </summary>
    public class PipelineManager : MonoBehaviour
    {
        void Start()
        {
            SeaManager.Instance.Init();
        }
    }
}
