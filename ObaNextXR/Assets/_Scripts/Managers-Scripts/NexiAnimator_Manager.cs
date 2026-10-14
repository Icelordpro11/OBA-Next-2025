using System;
using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;

public class NexiAnimator_Manager : MonoBehaviour
{
    [SerializeField] private Animator _animator, _boxAnimator;
    private NavMeshAgent _navMeshAgent;
    private FSM_Controller _fsmController;

    [SerializeField] private string _currentAnim;

    private bool _isNexiInBox = true;
    [SerializeField] private Transform spawnPoint;

    void Start()
    {
        Animator[] _animators = transform.parent.GetComponentsInChildren<Animator>();
        _animator = _animators[0];
        _boxAnimator = _animators[2];

        _navMeshAgent = GetComponentInParent<NavMeshAgent>();
        _fsmController = GetComponentInParent<FSM_Controller>();
    }

    public async void SetNexiAnimation(string targetAnimationName, float transitionDuration)
    {
        if (_currentAnim == targetAnimationName) return;

        if (_currentAnim == "Nexi_EscapingBox")
        {
            _currentAnim = targetAnimationName;
            return;
        }

        if (_currentAnim == "Nexi_Walking" && (targetAnimationName == "Nexi_IdleStanding" || targetAnimationName == "Nexi_Listening"))
        {
            await UniTask.WaitUntil(() =>
            {
                float animClipTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
                return animClipTime <= 0.5f || animClipTime > 0.8f && _navMeshAgent.velocity == Vector3.zero;
            });
        }

        _currentAnim = targetAnimationName;
        _animator.CrossFade(_currentAnim, transitionDuration);
    }

    public void RegulateNexiIdleAnims(ref float playerStandingStillTimer)
    {
        if (_navMeshAgent.velocity == Vector3.zero)
        {
            if (playerStandingStillTimer > 3) return;

            playerStandingStillTimer += Time.deltaTime;

            string idleAnim = playerStandingStillTimer >= 3 ? "Nexi_IdleSitting" : "Nexi_IdleStanding";
            SetNexiAnimation(idleAnim, 0.25f);

            return; // Prevent further animation updates
        }

        playerStandingStillTimer = 0;

        string movingAnim = _navMeshAgent.speed >= 2 ? "Nexi_Running" : "Nexi_Walking";
        SetNexiAnimation(movingAnim, 0.25f);
    }

    public void Nexi_PlayerInteract()
    {
        Action state = _isNexiInBox ? EscapeBox : _fsmController.PlayerFinishedRecording;
        state.Invoke();
    }

    private void EscapeBox()
    {
        _isNexiInBox = false;
        SetNexiAnimation("Nexi_EscapingBox", 0.25f);
        _boxAnimator.CrossFade("NexiBox_EscapingBox", 0.25f);
    }

    public void AdaptAnimationSpeed(string animation, float adaptedSpeed)
    {
        if (_currentAnim != animation) return;

        _animator.speed = adaptedSpeed;
    }

    private void TeleportToSpawn()
    {
        if (!_isNexiInBox) return;
        transform.parent.position = spawnPoint.position;
    }
}