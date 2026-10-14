using _Scripts.Managers_Scripts;
using UnityEngine;
using Artem_Library.Library_Scripts.Systems_Scripts.FSM_Scripts;
using UnityEngine.AI;
using Artem_Library.Attribute_Scripts;
using Cysharp.Threading.Tasks;
using Artem_Library.Library_Scripts.ScriptsTemplate_Scripts;

[RequireComponent(typeof(FSM_Controller))]
public class FSM_Controller : Singleton_MonoBehaviour<FSM_Controller>
{
    private Fsm _fsm;
    private NexiAnimator_Manager _nexiAnimatorMgr;
    [ExpandableScript] private NavMeshAgent _nexiNavAgent;
    public bool IsNexiWaiting;
    private bool _hasExecutedOnce;

    public Vector3 SelectedCategoryPos { get; set; }

    public Transform PlayerTransform { get; set; }
    public float NexiPlayerDistance { get; set; }

    private void OnValidate()
    {
        _fsm ??= new Fsm();
    }

    private void Start()
    {
        _fsm ??= new Fsm();

        Initialize_Variables();

        _nexiNavAgent.enabled = false;
        Initialize_FSM();
    }

    private void Initialize_Variables()
    {
        _nexiAnimatorMgr = GetComponentInChildren<NexiAnimator_Manager>();
        _nexiNavAgent = GetComponent<NavMeshAgent>();
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Initialize_FSM()
    {
        // Add States
        _fsm.AddState(new NexiInBox_State(_nexiAnimatorMgr));
        _fsm.AddState(new NexiListening_State(this, _fsm, _nexiNavAgent, _nexiAnimatorMgr, transform, PlayerTransform));
        _fsm.AddState(new NexiChatting_State(this, _fsm, _nexiNavAgent, _nexiAnimatorMgr, transform, PlayerTransform));

        _fsm.AddState(new NexiFollowing_State(this, _fsm, _nexiNavAgent, _nexiAnimatorMgr));
        _fsm.AddState(new NexiGuidingToWaypoint_State(this, _fsm, _nexiNavAgent, _nexiAnimatorMgr));


        // Define Start State
        _fsm.ChangeState<NexiInBox_State>();
    }

    public void Update() => _fsm.Update();
    public void FixedUpdate() => _fsm.FixedUpdate();
    public void LateUpdate() => _fsm.LateUpdate();

    public async UniTaskVoid CheckNexiPlayerDistance()
    {
        Vector3 savedPlayerPos = PlayerTransform.position;
        await UniTask.DelayFrame(1);

        float playerFrameDistance = Vector3.Distance(savedPlayerPos, PlayerTransform.position);
        if (playerFrameDistance > 2) return;

        float newDistance = Vector3.Distance(PlayerTransform.position, transform.position);
        NexiPlayerDistance = newDistance;
    }

    public void CheckPlayerGoingAway(float maxPlayerDistance)
    {
        if (NexiPlayerDistance < maxPlayerDistance) return;

        _fsm.ChangeState<NexiFollowing_State>();
    }

    public void SetNexiListeningState()
    {
        _fsm.ChangeState<NexiListening_State>();
    }

    public void PlayerFinishedRecording()
    {
        _fsm.ChangeState<NexiChatting_State>();
    }

    public void SetCategoryNexi(Transform shelfMark)
    {
        SelectedCategoryPos = shelfMark.position;
        _fsm.ChangeState<NexiGuidingToWaypoint_State>();
    }

    public void NexiSpeechandText(string sentence)
    {
        IsNexiWaiting = false;

        if (_hasExecutedOnce) return;
        
        _hasExecutedOnce = true;
        SentenceSender(sentence).Forget();
    }

    private async UniTaskVoid SentenceSender(string sentence)
    {
        await OpenAITTS_System.Instance.Speak_Async(sentence);
        _Scripts.NexiUI_System.Instance.SetDialogueText(sentence);
    }

    public void ResetNexiWaiting()
    { 
        IsNexiWaiting = false;
    }

    public void ActivateMauwWatch()
    {
        GameManager.Instance.EnableMauwWatch(true);
        _nexiNavAgent.enabled = true;
    }
}