using _Scripts;
using UnityEngine;
using Artem_Library.Library_Scripts.Systems_Scripts.FSM_Scripts;
using UnityEngine.AI;

public class NexiGuidingToWaypoint_State : FsmsState, IEnterState, IUpdateState
{
    private readonly FSM_Controller _fsmController;
    private readonly NavMeshAgent _meshAgent;
    private readonly Fsm _fsm;
    private readonly NexiAnimator_Manager _nexiAnimMgr;
    
    private Vector3 _dest;
    
    private float _noPlayerMovementTimer;
    bool _hasReachedDestination;

    public NexiGuidingToWaypoint_State(FSM_Controller controller, Fsm fsm, NavMeshAgent agent, NexiAnimator_Manager nexiAnimationManager)
    {
        _fsmController = controller;
        _fsm = fsm;
        _meshAgent = agent;
        _nexiAnimMgr = nexiAnimationManager;
    }

    public void Enter()
    {
        _meshAgent.stoppingDistance = 0;
        _dest = _fsmController.SelectedCategoryPos;
        GoTowardsCategoryWaypoint();
    }

    public void Update()
    {
        UpdateNexiNavMesh();
        CheckIfDestinationReached();
    }

    private void UpdateNexiNavMesh()
    {
        _fsmController.CheckNexiPlayerDistance();
        AdaptNexiSpeed();
        _nexiAnimMgr.RegulateNexiIdleAnims(ref _noPlayerMovementTimer);

        _fsmController.CheckPlayerGoingAway(6f);
    }

    private void GoTowardsCategoryWaypoint()
    {
        _meshAgent.SetDestination(_dest);
    }

    private void CheckIfDestinationReached()
    {
        _hasReachedDestination = Vector3.Distance(_dest, _fsmController.transform.position) < 1f;

        if (_hasReachedDestination && _fsmController.NexiPlayerDistance < 3f)
        {
            _fsmController.IsNexiWaiting = true;
            _fsm.ChangeState<NexiChatting_State>();
        }
    }

    private void AdaptNexiSpeed()
    {
        if (_hasReachedDestination)
        {
            _meshAgent.speed = 0;
            return;
        }

        //Adapts Nexi's Speed based on length NexiPlayerDifference
        float nexiAdaptedSpeed = _fsmController.NexiPlayerDistance switch
        {
            > 4 => 0f,
            > 2.5f and < 5 => 1.25f,
            _ => 2.5f,
        };

        _meshAgent.speed = nexiAdaptedSpeed;
    }


}