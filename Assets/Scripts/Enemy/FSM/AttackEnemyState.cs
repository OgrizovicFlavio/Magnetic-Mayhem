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
    }

    public override void OnUpdate()
    {
        var controller = context.GetController();

        controller.AttackPlayer();

        if (!controller.IsPlayerInAttackRange())
        {
            if (controller.IsPlayerInChaseRange())
                context.ChangeState(EnemyState.Chase);
            else
                context.ChangeState(EnemyState.Patrol);
        }
    }

    public override void OnExit() { }
}
