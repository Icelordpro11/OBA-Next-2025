using UnityEngine;
using Artem_Library.Library_Scripts.Systems_Scripts.FSM_Scripts;

public class NexiInBox_State : FsmsState
{
    private readonly NexiAnimator_Manager _nexiAnimMgr;

    public NexiInBox_State(NexiAnimator_Manager nexiAnimationManager)
    {
        _nexiAnimMgr = nexiAnimationManager;
    }

    
}
