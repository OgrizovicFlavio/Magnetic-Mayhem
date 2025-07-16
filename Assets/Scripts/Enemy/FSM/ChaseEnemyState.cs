public class ChaseEnemyState : BaseEnemyState
{
    public ChaseEnemyState(EnemyFSM fsm) : base(fsm)
    {
        enemyState = EnemyState.Chase;
    }

    public override void OnEnter()
    {
        movement.StopMovement();
    }

    public override void OnUpdate()
    {
        if (controller.IsMagnetized())
        {
            context.ChangeState(EnemyState.Magnetized);
            return;
        }

        var target = controller.GetTarget();

        if (target != null)
        {
            movement.MoveTowards(target.position);
            movement.RotateTowards(target.position);
        }

        if (controller.IsPlayerInAttackRange())
            context.ChangeState(EnemyState.Attack);
        else if (!controller.IsPlayerInChaseRange())
            context.ChangeState(EnemyState.Patrol);
    }

    public override void OnExit() { }
}
