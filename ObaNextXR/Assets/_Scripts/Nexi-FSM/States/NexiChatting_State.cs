using UnityEngine;
using Artem_Library.Library_Scripts.Systems_Scripts.FSM_Scripts;
using UnityEngine.AI;
using System.Collections;
using System;

public class NexiChatting_State : FsmsState, IEnterState, IUpdateState
{
    private readonly FSM_Controller _fsmController;
    private readonly Fsm _fsm;
    private readonly NexiAnimator_Manager _nexiAnimMgr;
    private readonly NavMeshAgent _meshAgent;

    private readonly Transform _transform;
    private readonly Transform _playerTransform;

    private float _playerNoTalkingTimer;

    public NexiChatting_State(FSM_Controller controller, Fsm fsm, NavMeshAgent nexiNavAgent, NexiAnimator_Manager nexiAnimationManager, Transform nexiTransform, Transform playerTransform)
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
        _nexiAnimMgr.SetNexiAnimation("Nexi_IdleStanding", 0.25f);

        _playerNoTalkingTimer = 0;
    }

    public void Update()
    {
        _nexiAnimMgr.RegulateNexiIdleAnims(ref _playerNoTalkingTimer);

        if (!_fsmController.IsNexiWaiting)
        {
            _fsmController.CheckNexiPlayerDistance();
            _fsmController.CheckPlayerGoingAway(3f);
        }
    }
}
