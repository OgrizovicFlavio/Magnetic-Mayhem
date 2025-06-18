public abstract class BasePlayerState : BaseState<PlayerFSM>
{
    protected PlayerFSM fsm;

    public PlayerState playerState { get; protected set; }

    public BasePlayerState(PlayerFSM fsm) : base(fsm)
    {
        this.fsm = fsm;
    }
}
