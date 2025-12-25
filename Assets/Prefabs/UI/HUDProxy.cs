using Terrain.Demo;
using UI.HUD;
using UnityEngine;
using Compass = UI.HUD.Compass;

namespace Prefabs.UI
{
    /// <summary>
    /// Class for passing values into HUD
    /// </summary>
    public class HUDProxy : MonoBehaviour
    {
        [SerializeField] Rigidbody _rigidbody;
        [SerializeField] private CameraController _controller;
        [SerializeField] private Tape _heightTape;
        [SerializeField] private Tape _speedTape;
        [SerializeField] private Compass _compass;

        void Update()
        {
            _heightTape.value = _rigidbody.transform.position.y;
            _speedTape.value = _rigidbody.linearVelocity.magnitude;
            _compass.degrees = _controller.GetCardinalDirection;
        }
    }
}
