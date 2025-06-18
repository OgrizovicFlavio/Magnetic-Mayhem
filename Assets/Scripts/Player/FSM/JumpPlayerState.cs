public class JumpPlayerState : BasePlayerState
{
    public JumpPlayerState(PlayerFSM fsm) : base(fsm)
    {
        playerState = PlayerState.Jump;
    }

    public override void OnEnter()
    {
        fsm.GetController().Jump();
        fsm.ChangeState(PlayerState.Idle);
    }

    public override void OnUpdate() { }

    public override void OnExit() { }
}
