using System;
using Artem_Library.Attribute_Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Weight_Controller : MonoBehaviour
{
    [Header("Platform Settings")] 
    [SerializeField] private Transform leftSide;
    [SerializeField] private Transform rightSide;
    [SerializeField] private LayerMask blockLayer;
    
    [Header("Detection Size")]
    [SerializeField] private Vector3 boxSize = new(2, 2, 2);
    [SerializeField] private Vector3 boxOffset = Vector3.zero;
    
    [field: SerializeField, BoolConverter] public bool HasFallen { get; private set; }

    [SerializeField] private UnityEvent onLeftBlockFellEvent, onRightBlockFellEvent;

    private UniTaskVoid detectionTask;

    private void Start() => RestartDetection();

    public void ResetWeight_System(bool resetSystem)
    {
        HasFallen = resetSystem;
        RestartDetection();
    }

    private void RestartDetection()
    {
        HasFallen = false;
        detectionTask = CheckBlocksLoop();
    }

    private async UniTaskVoid CheckBlocksLoop()
    {
        while (!HasFallen)
        {
            await UniTask.Delay(50);

            var leftHasBlocks = Physics.BoxCast(leftSide.position + boxOffset, boxSize / 2, Vector3.down, Quaternion.identity, 1f, blockLayer);
            var rightHasBlocks = Physics.BoxCast(rightSide.position + boxOffset, boxSize / 2, Vector3.down, Quaternion.identity, 1f, blockLayer);

            if (leftHasBlocks && rightHasBlocks) continue;

            HasFallen = true;

            if (!leftHasBlocks)
            {
                onLeftBlockFellEvent?.Invoke();
                Debug.Log("Left Block");
            }
            else
            {
                onRightBlockFellEvent?.Invoke();
                Debug.Log("Right Block");
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (leftSide is not null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(leftSide.position + boxOffset + Vector3.down * 0.5f, boxSize);
        }

        if (rightSide is null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(rightSide.position + boxOffset + Vector3.down * 0.5f, boxSize);
    }
}
