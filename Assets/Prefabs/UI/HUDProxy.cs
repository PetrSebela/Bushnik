using UI.HUD;
using UnityEngine;

namespace Prefabs.UI
{
    /// <summary>
    /// Class for passing values into HUD
    /// </summary>
    public class HUDProxy : MonoBehaviour
    {
        [SerializeField] Rigidbody _rigidbody;
        [SerializeField] private Tape _heightTape;
        [SerializeField] private Tape _speedTape;
        
        void Update()
        {
            _heightTape.value = _rigidbody.transform.position.y;
            _speedTape.value = _rigidbody.linearVelocity.magnitude;
        }
    }
}
