using System;
using Artem_Library.Attribute_Scripts;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class Timer_System : MonoBehaviour
{
    private enum ActivationMode { Enable, Disable, Destroy }

    [Header("Timer Settings")]
    [SerializeField] private float countdownTime = 5f;
    [SerializeField] private bool startOnEnable = true;
    [SerializeField] private ActivationMode activationMode = ActivationMode.Enable;

    [Header("Events")]
    [SerializeField] private UnityEvent onTimerEnd; 
    [field: SerializeField] public float RemainingTime { get; private set; }
    public bool IsRunning { get; private set; }

    private void OnEnable()
    {
        if (startOnEnable)
            StartTimer();
    }

    private void Start()
    {
        if (!startOnEnable) 
            StartTimer();
    }
    [ContextMenu("Start Timer")]
    public void StartTimer()
    {
        if (IsRunning) return;

        RemainingTime = countdownTime;
        IsRunning = true;
        CountdownTask().Forget();
    }
    
    private async UniTaskVoid CountdownTask()
    {
        while (RemainingTime > 0f && IsRunning)
        {
            var waitTime = Mathf.Min(1f, RemainingTime);
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime));

            RemainingTime -= waitTime;
        }

        if (!IsRunning) return;

        IsRunning = false;
        onTimerEnd?.Invoke();
        HandleObjectActivation();
    }

    private void HandleObjectActivation()
    {
        switch (activationMode)
        {
            case ActivationMode.Enable:
                gameObject.SetActive(true);
                break;
            case ActivationMode.Disable:
                gameObject.SetActive(false);
                break;
            case ActivationMode.Destroy:
                Destroy(gameObject);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    [ContextMenu("Stop Timer")]
    public void StopTimer() => IsRunning = false;

    [ContextMenu("Reset Timer")]
    public void ResetTimer()
    {
        StopTimer();
        RemainingTime = countdownTime;
    }
}
