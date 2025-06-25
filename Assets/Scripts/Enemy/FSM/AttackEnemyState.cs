using UnityEngine;

public class AttackEnemyState : BaseEnemyState
{
    public AttackEnemyState(EnemyFSM fsm) : base(fsm)
    {
        enemyState = EnemyState.Attack;
    }

    public override void OnEnter()
    {
        Debug.Log("Enemy entered ATTACK state");
        movement.StopMovement();
    }

    public override void OnUpdate()
    {
        if (controller.IsMagnetized())
        {
            context.ChangeState(EnemyState.Magnetized);
            return;
        }

        if (!attack.IsRetreating())
        {
            attack.TryAttack();
        }

        if (!attack.IsRetreating() && !controller.IsPlayerInAttackRange())
        {
            if (controller.IsPlayerInChaseRange())
                context.ChangeState(EnemyState.Chase);
            else
                context.ChangeState(EnemyState.Patrol);
        }
    }

    public override void OnExit() { }
}
