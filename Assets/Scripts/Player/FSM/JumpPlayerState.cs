using UnityEngine;

public class JumpPlayerState : BasePlayerState
{
    private float jumpTimer = 0f;
    private float minJumpDuration = 0.5f;

    public JumpPlayerState(PlayerFSM fsm) : base(fsm)
    {
        playerState = PlayerState.Jump;
    }

    public override void OnEnter()
    {
        fsm.GetController().GetAnimator().SetInteger("State", (int)PlayerState.Jump);
        fsm.GetController().Jump();
        jumpTimer = 0f;
    }

    public override void OnUpdate()
    {
        jumpTimer += Time.deltaTime;

        if (fsm.GetController().IsGrounded() && jumpTimer > minJumpDuration)
        {
            fsm.ChangeState(PlayerState.Idle);
        }
    }

    public override void OnExit() { }
}
