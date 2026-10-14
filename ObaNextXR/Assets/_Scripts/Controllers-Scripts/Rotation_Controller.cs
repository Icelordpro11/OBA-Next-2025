using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Rotation_Controller : MonoBehaviour
{
    [SerializeField] private float _accelerationTime;
    
    [SerializeField] private float _speed = 100;
    private bool _isRotating = false;
    
    private CancellationTokenSource cts;


    private void OnEnable()
    {
        cts = new CancellationTokenSource();
    }
    private void OnDisable()
    {
       cts.Cancel();
    }

    private void Start()
    {
        Rotationloop(cts.Token).Forget();
    }

    public void StartRotation()
    {
        cts = new CancellationTokenSource();
        Rotationloop(cts.Token).Forget();
    }

    public void StopRotation()
    {
        cts?.Cancel();
        cts = null;
    }

    private async UniTaskVoid Rotationloop(CancellationToken token)
    {
        float currentSpeed = 0;
        float elapsed = 0;

        while (elapsed < _accelerationTime && !token.IsCancellationRequested)
        {
            currentSpeed = Mathf.Lerp(0, _speed, elapsed / _accelerationTime);
            transform.Rotate(Vector3.up, currentSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
        currentSpeed = _speed;
        
        while (!token.IsCancellationRequested)
        {
            transform.Rotate(Vector3.up, Time.deltaTime * _speed);
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }
}
