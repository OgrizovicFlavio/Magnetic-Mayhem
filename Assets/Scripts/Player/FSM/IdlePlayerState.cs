using UnityEngine;

public class IdlePlayerState : BasePlayerState
{
    public IdlePlayerState(PlayerFSM fsm) : base(fsm)
    {
        playerState = PlayerState.Idle;
    }

    public override void OnEnter()
    {
        fsm.GetController().GetAnimator().SetInteger("State", (int)PlayerState.Idle);

        fsm.GetController().SetMoveInput(Vector2.zero);
    }

    public override void OnUpdate()
    {
        var input = fsm.GetInput();
        Vector2 move = input.GetMoveInput();

        if (move.magnitude > 0.1f)
        {
            fsm.ChangeState(PlayerState.Run);
        }
        else if (input.IsJumping())
        {
            fsm.ChangeState(PlayerState.Jump);
        }
    }

    public override void OnExit() { }
}
