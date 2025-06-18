using System;

public class ShootPlayerState : BasePlayerState
{
    private Action shootAction;

    public ShootPlayerState(PlayerFSM fsm, Action shootAction) : base(fsm)
    {
        this.shootAction = shootAction;
        playerState = PlayerState.Shoot;
    }

    public override void OnEnter()
    {
        shootAction?.Invoke();
        fsm.ChangeState(PlayerState.Idle);
    }

    public override void OnUpdate() { }

    public override void OnExit() { }
}

