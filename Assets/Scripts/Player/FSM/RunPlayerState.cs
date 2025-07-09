using UnityEngine;

public class RunPlayerState : BasePlayerState
{
    public RunPlayerState(PlayerFSM fsm) : base(fsm)
    {
        playerState = PlayerState.Run;
    }

    public override void OnEnter()
    {
        fsm.GetController().GetAnimator().SetInteger("State", (int)PlayerState.Run);
    }

    public override void OnUpdate()
    {
        var input = fsm.GetInput();
        var controller = fsm.GetController();
        Vector2 move = input.GetMoveInput();

        controller.SetMoveInput(move);

        if (move.magnitude < 0.1f)
        {
            fsm.ChangeState(PlayerState.Idle);
        }
        else if (input.IsJumping())
        {
            fsm.ChangeState(PlayerState.Jump);
        }
    }

    public override void OnExit()
    {
        
    }
}
