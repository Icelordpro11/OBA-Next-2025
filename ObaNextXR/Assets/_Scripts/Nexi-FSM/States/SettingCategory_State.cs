using UnityEngine;
using Artem_Library.Library_Scripts.Systems_Scripts.FSM_Scripts;
using System.Collections.Generic;
using System.Linq;

public class SettingCategory_State : FsmsState, IEnterState, IUpdateState
{
    private readonly FSM_Controller _fsmController;
    private readonly Fsm _fsm;

    private readonly List<Transform> _categoriesList;

    public SettingCategory_State(FSM_Controller controller, Fsm fsm, List<Transform> categoriesList)
    {
        _fsmController = controller;
        _fsm = fsm;

        _categoriesList = categoriesList;
    }

    public void Enter()
    {
        //_fsmController.FocussedCategory = _categoriesList[0];
        //_fsm.ChangeState<NexiGuidingToWaypoint_State>();
       // _fsmController.SetBookInfo("Dromen");
    }

    public void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    _fsmController.FocussedCategory = _categoriesList[0];
        //    _fsm.ChangeState<NexiGuidingToWaypoint_State>();
        //}

        //if (Input.GetKeyDown(KeyCode.Backspace))
        //{
        //    _fsmController.FocussedCategory = _categoriesList[1];
        //    _fsm.ChangeState<NexiGuidingToWaypoint_State>();
        //}
    }

 
}

