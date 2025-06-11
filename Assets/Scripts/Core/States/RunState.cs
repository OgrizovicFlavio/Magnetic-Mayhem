using UnityEngine;

public class RunState : BaseState
{
    public RunState(PlayerFSM fsm) : base(fsm)
    {
        playerState = PlayerState.Run;
    }
    public override void OnEnter() { }
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
        else if (input.IsShooting())
        {
            fsm.ChangeState(PlayerState.Shoot);
        }
    }
    public override void OnExit() { }
}
