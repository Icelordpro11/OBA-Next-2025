using System;
using System.Collections.Generic;

namespace Artem_Library.Library_Scripts.Systems_Scripts.FSM_Scripts
{
    public class Fsm
    {
        private FsmsState _fsmsState;
        private readonly Dictionary<Type, FsmsState> _states = new Dictionary<Type, FsmsState>();

        public void AddState(FsmsState state)
        {
            var stateType = state.GetType();
            if (_states.ContainsKey(stateType)) return;
            _states[stateType] = state;
        }

        public void Update()
        {
            if (_fsmsState is IUpdateState updateState) updateState.Update();
        }

        
        public void FixedUpdate()
        {
            if (_fsmsState is IFixedUpdate fixedState) fixedState.FixedUpdate();
        }

        public void LateUpdate()
        {
            if (_fsmsState is ILateUpdateState fixedState) fixedState.LateUpdate();
        }

        public void ChangeState<T>() where T : FsmsState
        {
            var type = typeof(T);
            if (_fsmsState!= null && _fsmsState.GetType() == type) return;
            if (!_states.TryGetValue(type, out var newState)) return;
            if (_fsmsState is IExitState exitState) exitState.Exit();
            _fsmsState = newState;
            if (_fsmsState is IEnterState enterState) enterState.Enter();
        }
    }
}