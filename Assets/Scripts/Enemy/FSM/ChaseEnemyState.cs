using UnityEngine;

public class ChaseEnemyState : BaseEnemyState
{
    public ChaseEnemyState(EnemyFSM fsm) : base(fsm)
    {
        enemyState = EnemyState.Chase;
    }

    public override void OnEnter()
    {
        Debug.Log("Enemy entered CHASE state");
    }

    public override void OnUpdate()
    {
        var controller = context.GetController();
        var target = controller.GetTarget();

        if (target != null)
        {
            controller.MoveTowards(target.position);
        }

        if (controller.IsPlayerInAttackRange())
        {
            context.ChangeState(EnemyState.Attack);
        }
        else if (!controller.IsPlayerInChaseRange())
        {
            context.ChangeState(EnemyState.Patrol);
        }
    }

    public override void OnExit() { }
}
