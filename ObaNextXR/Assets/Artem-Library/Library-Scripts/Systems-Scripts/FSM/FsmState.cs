namespace Artem_Library.Library_Scripts.Systems_Scripts.FSM_Scripts
{
    public interface IFixedUpdate
    {
        void FixedUpdate();
    }

    public interface IUpdateState
    {
        void Update();
    }

    public interface ILateUpdateState
    {
        void LateUpdate();
    }

    public interface IEnterState
    {
        void Enter();
    }

    public interface IExitState
    {
        void Exit();
    }
}