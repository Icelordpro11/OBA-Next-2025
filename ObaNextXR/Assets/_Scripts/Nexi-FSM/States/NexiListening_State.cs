using UnityEngine;
using Artem_Library.Library_Scripts.Systems_Scripts.FSM_Scripts;
using UnityEngine.AI;
using System.Collections;
using System;

public class NexiListening_State : FsmsState, IEnterState, IUpdateState
{
    private readonly FSM_Controller _fsmController;
    private readonly Fsm _fsm;
    private readonly NexiAnimator_Manager _nexiAnimMgr;
    private readonly NavMeshAgent _meshAgent;

    private readonly Transform _transform;
    private readonly Transform _playerTransform;

    public NexiListening_State(FSM_Controller controller, Fsm fsm, NavMeshAgent nexiNavAgent, NexiAnimator_Manager nexiAnimationManager, Transform nexiTransform, Transform playerTransform)
    {
        _fsmController = controller;
        _fsm = fsm;
        _meshAgent = nexiNavAgent;
        _nexiAnimMgr = nexiAnimationManager;

        _transform = nexiTransform;
        _playerTransform = playerTransform;
    }

    public void Enter()
    {
        _meshAgent.speed = 0;
        _fsmController.IsNexiWaiting = false;
        _nexiAnimMgr.SetNexiAnimation("Nexi_Listening", 0.25f);
        _fsmController.StartCoroutine(LookAtPlayer()); 
    }

    public void Update()
    {
        UpdateNexiNavMesh();

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            _fsm.ChangeState<SettingCategory_State>();
        }
    }

    private void UpdateNexiNavMesh()
    {
        _fsmController.CheckNexiPlayerDistance();
        _fsmController.CheckPlayerGoingAway(3f);
    }

    private IEnumerator LookAtPlayer()
    {
        yield return new WaitForEndOfFrame();
        Vector3 direction = _playerTransform.position - _transform.position;
        direction.y = 0f; // Ignore vertical difference

        //  if (direction.sqrMagnitude < 0.001f) return; // Avoid zero-length direction vector
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        while (Quaternion.Angle(_transform.rotation, targetRotation) > 0.1f)
        {
            _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, 2f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
