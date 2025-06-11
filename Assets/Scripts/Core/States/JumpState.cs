using UnityEngine;

public class JumpState : BaseState
{
    public JumpState(PlayerFSM fsm) : base(fsm)
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
