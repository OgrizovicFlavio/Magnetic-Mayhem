using UnityEngine;

public class ShootState : BaseState
{
    public ShootState(PlayerFSM fsm) : base(fsm)
    {
        playerState = PlayerState.Shoot;
    }

    public override void OnEnter()
    {
        fsm.GetShooter().TryShoot();
        fsm.ChangeState(PlayerState.Idle);
    }

    public override void OnUpdate() { }

    public override void OnExit() { }
}
