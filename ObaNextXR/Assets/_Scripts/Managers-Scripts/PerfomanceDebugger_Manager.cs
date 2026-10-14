using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using System.Diagnostics;
using Artem_Library.Library_Scripts.ScriptsTemplate_Scripts;

public class PerfomanceDebugger_Manager : Singleton_MonoBehaviour<PerfomanceDebugger_Manager>
{
    
    [Header("Settings Values")]
    [SerializeField, Range(0,1)] private float updateInterval = 0.25f;
    [SerializeField] private TextMeshProUGUI debugText;

    private const bool IsActive = true;
    private float _deltaTime;
    private Stopwatch _stopwatch;

    private void Awake()
    {
        _stopwatch = new Stopwatch();
    }

    private void Start() => PerformanceMonitor_Loop().Forget();

    private void Update() => _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;

    private async UniTaskVoid PerformanceMonitor_Loop()
    {
        while (true)
        {
            if (IsActive)
            {
                _stopwatch.Restart();
                UpdatePerformance_Stats();
                _stopwatch.Stop();
            }
            await UniTask.Delay(TimeSpan.FromSeconds(updateInterval));
        }
    }
    private void UpdatePerformance_Stats()
    {
        var fps = 1.0f / _deltaTime;
        var totalMemoryLog = Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024);
        var objectCount = Get_ObjectCount();
        var cpuTime = _deltaTime * 1000f;
        var gpuTime = Get_GPUTime();
        float drawCalls = Get_DrawCallEstimate();
    
        var performanceStats =
            $"<b>Performance Stats</b>\n" +
            $"==================\n" +
            $"FPS: {fps:F1}\n" +
            $"Memory: {totalMemoryLog} MB\n" +
            $"Objects: {objectCount}\n";

        var renderingStats =
            $"\n<b>Rendering</b>\n" +
            $"==================\n" +
            $"DrawCalls {drawCalls} ms\n" +
            $"CPU: {cpuTime:F2} ms\n" +
            $"GPU: {gpuTime:F2} ms\n";

        debugText.text = performanceStats + renderingStats;
    }
    
    private static int Get_ObjectCount() => FindObjectsOfType<GameObject>().Length;
    
    private static int Get_DrawCallEstimate()
    {
        FrameTimingManager.CaptureFrameTimings();
        var frameTimings = new FrameTiming[1];
    
        if (FrameTimingManager.GetLatestTimings(1, frameTimings) > 0)
            return (int)frameTimings[0].gpuFrameTime; 
    
        return -1;
    }
    
    private static float Get_GPUTime()
    {
        var displaySubsystem = XRGeneralSettings.Instance?.Manager?.activeLoader?.GetLoadedSubsystem<XRDisplaySubsystem>();
        if (displaySubsystem == null) return 0f;

        XRStats.TryGetGPUTimeLastFrame( out var gpuTime);
        return gpuTime;
    }
}
