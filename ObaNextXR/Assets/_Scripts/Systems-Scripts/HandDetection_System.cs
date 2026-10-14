using System;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading;
using _Scripts.Managers_Scripts;
using Artem_Library.Attribute_Scripts;
using NaughtyAttributes;

namespace _Scripts
{
    public class HandDetection_System : MonoBehaviour
    {
        private enum DetectionShape { Sphere, Cube }
        
        [Header("Detection Settings")]
        [SerializeField] private DetectionShape detectionShape = DetectionShape.Sphere;
        [SerializeField, Range(0.1f, 10)] private float detectionRadius = 0.3f;
        [SerializeField] private Vector3 detectionCubeSize = new Vector3(0.5f, 0.5f, 0.5f);
        
        [Header("Timer Settings")]
        [SerializeField, BoolConverter] private bool enableTimer;
        [SerializeField] private float enterDelay = 0.5f;
        [SerializeField] private float exitDelay = 0.5f;
        
        [SerializeField] private UnityEvent onHandEnterEvent, onHandExitEvent;
        
        private Transform _rightHandTarget, _leftHandTarget;

        private CancellationTokenSource _cts;
        private CancellationTokenSource _enterCts, _exitCts;

        [field: SerializeField, BoolConverter] public bool HandInside { get; private set; }
        
        private void Awake()
        {
            _rightHandTarget = GameManager.Instance.RightHandTarget;
            _leftHandTarget = GameManager.Instance.LeftHandTarget;
        }

        private void Start()
        {
            _cts = new CancellationTokenSource();
            CheckHandInsideLoop(_cts.Token).Forget();
        }

        private async UniTaskVoid CheckHandInsideLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var isAnyHandInside = IsHandInside(_rightHandTarget) || IsHandInside(_leftHandTarget);

                switch (isAnyHandInside)
                {
                    case true when !HandInside:
                    {
                        _exitCts?.Cancel();
                        _enterCts = new CancellationTokenSource();
                        if (enableTimer) await UniTask.Delay((int)(enterDelay * 1000), cancellationToken: _enterCts.Token);
                        if (!_enterCts.Token.IsCancellationRequested)
                        {
                            HandInside = true;
                            onHandEnterEvent?.Invoke();
                        }
                        break;
                    }
                    case false when HandInside:
                    {
                        _enterCts?.Cancel(); 
                        _exitCts = new CancellationTokenSource();
                        if (enableTimer) await UniTask.Delay((int)(exitDelay * 1000), cancellationToken: _exitCts.Token);
                        if (!_exitCts.Token.IsCancellationRequested)
                        {
                            HandInside = false;
                            onHandExitEvent?.Invoke();
                        }
                        break;
                    }
                }
                
                await UniTask.Delay(50, cancellationToken: token);
            }
        }

        private bool IsHandInside(Transform handTarget)
        {
            if (handTarget is null || !handTarget.CompareTag("Hand")) return false;
            switch (detectionShape)
            {
                case DetectionShape.Sphere:
                    return Vector3.Distance(transform.position, handTarget.position) < detectionRadius;
                    
                case DetectionShape.Cube:
                    var halfSize = detectionCubeSize * 0.5f;
                    var localPos = transform.InverseTransformPoint(handTarget.position);
                    return Mathf.Abs(localPos.x) <= halfSize.x &&
                           Mathf.Abs(localPos.y) <= halfSize.y &&
                           Mathf.Abs(localPos.z) <= halfSize.z;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            
            switch (detectionShape)
            {
                case DetectionShape.Sphere:
                    Gizmos.DrawWireSphere(transform.position, detectionRadius);
                    break;
                
                case DetectionShape.Cube:
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawWireCube(Vector3.zero, detectionCubeSize);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _enterCts?.Cancel();
            _exitCts?.Cancel();
        }

        public void StopHandDetection()
        {
            _cts?.Cancel();
            _enterCts?.Cancel();
            _exitCts?.Cancel();
        }
        
        public void EnableHandDetection()
        {
            // Cancel any pending enter/exit delays
            _enterCts?.Cancel();
            _exitCts?.Cancel();
            
            HandInside = false;

            // Start a new detection loop
            _cts = new CancellationTokenSource();
            CheckHandInsideLoop(_cts.Token).Forget();
        }
    }
}