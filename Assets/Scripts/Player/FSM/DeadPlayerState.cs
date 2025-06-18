public class DeadPlayerState : BasePlayerState
{
    public DeadPlayerState(PlayerFSM fsm) : base(fsm)
    {
        playerState = PlayerState.Dead;
    }

    public override void OnEnter() { }

    public override void OnUpdate() { }

    public override void OnExit() { }
}

