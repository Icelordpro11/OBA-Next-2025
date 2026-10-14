using UnityEngine;
using Artem_Library.Library_Scripts.Systems_Scripts.FSM_Scripts;
using UnityEngine.AI;

public class NexiFollowing_State : FsmsState, IEnterState, ILateUpdateState, IUpdateState
{
    private readonly FSM_Controller _fsmController;
    private readonly Fsm _fsm;
    private NavMeshAgent _meshAgent;
    private readonly NexiAnimator_Manager _nexiAnimMgr;

    private float _noPlayerMovementTimer;

    private Vector3 _navMeshDestPos;
    private Vector3 _playerPos;

    public NexiFollowing_State(FSM_Controller controller, Fsm fsm, NavMeshAgent agent, NexiAnimator_Manager nexiAnimationManager)
    {
        _fsmController = controller;
        _fsm = fsm;
        _meshAgent = agent;
        _nexiAnimMgr = nexiAnimationManager;
    }

    public void Enter()
    {
        _meshAgent.stoppingDistance = 0.5f;
        _meshAgent.SetDestination(_meshAgent.transform.position);
    }

    public void Update() => UpdateNexiNavMesh();
    public void LateUpdate() => UpdateVariables();

    private void UpdateVariables()
    {
        _navMeshDestPos = _meshAgent.destination;
        
        _playerPos = _fsmController.PlayerTransform.position;
        _playerPos.y = _navMeshDestPos.y;
    }

    private void UpdateNexiNavMesh()
    {
        _fsmController.CheckNexiPlayerDistance();
        AdaptNexiSpeed();

        _nexiAnimMgr.RegulateNexiIdleAnims(ref _noPlayerMovementTimer);

        GoTowardsPlayer();
    }

    private void GoTowardsPlayer()
    {
        if (!HasPlayerMoved()) return;
        _meshAgent.SetDestination(_playerPos);
    }

    private bool HasPlayerMoved()
    {
        return _navMeshDestPos != _playerPos;
    }

    private void AdaptNexiSpeed()
    {
        //Adapts Nexi's Speed based on length NexiPlayerDifference
        float nexiAdaptedSpeed = _fsmController.NexiPlayerDistance switch
        {
            > 4 => 3f,
            > 2 and < 4 => _fsmController.NexiPlayerDistance / 2,
            _ => 0f,
        };
     
        _meshAgent.speed = nexiAdaptedSpeed;
      //  _nexiAnimMgr.AdaptAnimationSpeed("Nexi_Walking", nexiAdaptedSpeed * 2);
    }
}

