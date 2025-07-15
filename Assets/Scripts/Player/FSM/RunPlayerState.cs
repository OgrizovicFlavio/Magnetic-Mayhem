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

        Transform cam = controller.GetCameraHolder();
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 worldMoveDir = camRight * move.x + camForward * move.y;
        Vector3 localMoveDir = controller.transform.InverseTransformDirection(worldMoveDir);

        controller.GetAnimator().SetFloat("MoveX", localMoveDir.x);
        controller.GetAnimator().SetFloat("MoveY", localMoveDir.z);

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
        var controller = fsm.GetController();
        controller.GetAnimator().SetFloat("MoveX", 0f);
        controller.GetAnimator().SetFloat("MoveY", 0f);
    }
}
