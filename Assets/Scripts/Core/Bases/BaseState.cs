public abstract class BaseState
{
    public PlayerState playerState = PlayerState.None;
    protected PlayerFSM fsm;

    public BaseState(PlayerFSM fsm)
    {
        this.fsm = fsm;
    }

    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnExit() { }
}
