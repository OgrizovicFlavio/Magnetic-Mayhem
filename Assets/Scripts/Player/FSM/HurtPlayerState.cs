public class HurtPlayerState : BasePlayerState
{
    public HurtPlayerState(PlayerFSM fsm) : base(fsm)
    {
        playerState = PlayerState.Hurt;
    }

    public override void OnEnter() { }

    public override void OnUpdate() { }

    public override void OnExit() { }
}

