using System.Timers;
using Aircraft.Controller.Links;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Aircraft.Controller
{
    public class FlapInput : InputLink
    {
        [SerializeField] private InputActionReference action;
        [SerializeField] private float[] _indents = {0};
        [SerializeField] private float _smoothing = 1;
        
        private int _indentIndex = 0;
        private float _targetAngle = 0;
        private float _flapAngle = 0;
        
        void Awake()
        {
            action.action.performed += ctx => IncrementFlap();
        }

        void Update()
        {
            _flapAngle = Mathf.Lerp(_flapAngle, _targetAngle,  _smoothing * Time.deltaTime);    
        }

        void IncrementFlap()
        {
            _indentIndex = (_indentIndex + 1) % _indents.Length;   
            _targetAngle = _indents[_indentIndex];
        }
        
        public override float GetOutput()
        {
            return _flapAngle;
        }
    }
}
