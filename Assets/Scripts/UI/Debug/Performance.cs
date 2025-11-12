using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace UI.Debug
{
    public class Performance : MonoBehaviour
    {
        private int _samples = 30;
        private List<float> _deltas = new List<float>();
        [SerializeField] private TMP_Text fpsDisplay;

        private void Start()
        {
            // Initialize sample buffer
            for (int i = 0; i < _samples; i++)
                _deltas.Add(0);
        }

        // Update is called once per frame
        void Update()
        {
            _deltas.RemoveAt(0);
            _deltas.Add(Time.unscaledDeltaTime);
            
            float fps = 1f / _deltas.Average();
            fpsDisplay.SetText($"FPS: {(int)fps}");
        }
    }
}
