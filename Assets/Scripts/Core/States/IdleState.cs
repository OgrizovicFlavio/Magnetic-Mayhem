using UnityEngine;

public class IdleState : BaseState
{
    public IdleState(PlayerFSM fsm) : base(fsm)
    {
        playerState = PlayerState.Idle;
    }

    public override void OnEnter()
    {
        fsm.GetController().SetMoveInput(Vector2.zero);
    }

    public override void OnUpdate()
    {
        var input = fsm.GetInput();
        var controller = fsm.GetController();
        Vector2 move = input.GetMoveInput();

        if (move.magnitude > 0.1f)
        {
            controller.SetMoveInput(move);
            fsm.ChangeState(PlayerState.Run);
        }
        else if (input.IsJumping())
        {
            fsm.ChangeState(PlayerState.Jump);
        }
        else if (input.IsShooting())
        {
            fsm.ChangeState(PlayerState.Shoot);
        }
    }

    public override void OnExit() { }
}
