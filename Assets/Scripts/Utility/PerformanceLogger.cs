using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Terrain;
using Unity.Profiling.Memory;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

namespace Utility
{
    public class PerformanceLogger : MonoBehaviour
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        private StreamWriter _logFile;
        private readonly List<float> _frameTimes = new();
        private double _loadingStartTime;
        
        void Awake()
        {
            var logFilePath = Path.Combine(Application.dataPath, "performance.log");
            _logFile = new StreamWriter(logFilePath);
            _logFile.WriteLine($"CPU: {SystemInfo.processorModel} ( {SystemInfo.processorCount} cores @ {SystemInfo.processorFrequency} MHz )");
            _logFile.WriteLine($"GPU: {SystemInfo.graphicsDeviceName} ( {SystemInfo.graphicsMemorySize} MB )");
            _logFile.WriteLine($"Memory: {SystemInfo.systemMemorySize} MB");
            _logFile.WriteLine();
            _loadingStartTime = Time.realtimeSinceStartup;
            Loader.Instance.afterLoading.AddListener(StartLogging);
        }

        void StartLogging()
        {
            _logFile.WriteLine($"Loaded in: {Time.realtimeSinceStartup - _loadingStartTime}");
            _logFile.WriteLine();
            _logFile.WriteLine("=== performance log ===");
            _logFile.WriteLine("avg_frame_time, min_frame_time, max_frame_time, memory_usage");
            _frameTimes.Clear();
            InvokeRepeating(nameof(Log), 1, 1);
        }

        void OnDestroy()
        {
            _logFile.Close();
        }

        void Update()
        {
            _frameTimes.Add(Time.unscaledDeltaTime);
        }
        
        void Log()
        {
            if(_frameTimes.Count == 0)
                return;
            
            var avg = _frameTimes.Sum() / _frameTimes.Count;
            var min = _frameTimes.Min();
            var max = _frameTimes.Max();
            var memory = Profiler.GetTotalReservedMemoryLong();
            
            _logFile.WriteLine($"{avg},{min},{max},{memory}");
            _frameTimes.Clear();
        }
        #endif
    }
}
