public class MagnetizedEnemyState : BaseEnemyState
{
    public MagnetizedEnemyState(EnemyFSM fsm) : base(fsm)
    {
        enemyState = EnemyState.Magnetized;
    }

    public override void OnEnter()
    {
        movement.StopMovement();
    }

    public override void OnUpdate()
    {
        if (controller.IsMagnetized())
            return;

        if (controller.HasCollidedWhileMagnetized())
        {
            bool isHeightRecovered = controller.UpdateHeight(0.2f);

            if (isHeightRecovered)
            {
                controller.ResetCollisionFlag();
                context.ChangeState(context.FindNextState().GetStateType());
            }
        }
    }

    public override void OnExit()
    {
        movement.StopMovement();
    }
}
