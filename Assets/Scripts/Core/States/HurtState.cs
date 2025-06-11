public class HurtState : BaseState
{
    public HurtState(PlayerFSM fsm) : base(fsm)
    {
        playerState = PlayerState.Hurt;
    }
    public override void OnEnter() { }
    public override void OnUpdate() { }
    public override void OnExit() { }
}
