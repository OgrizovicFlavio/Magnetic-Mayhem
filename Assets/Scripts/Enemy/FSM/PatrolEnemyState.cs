using UnityEngine;

public class PatrolEnemyState : BaseEnemyState
{
    private Transform currentPatrolTarget;

    public PatrolEnemyState(EnemyFSM fsm) : base(fsm)
    {
        enemyState = EnemyState.Patrol;
    }

    public override void OnEnter()
    {
        Debug.Log("Enemy entered PATROL state");

        var controller = context.GetController();

        controller.MoveToNextPatrolPoint();
        currentPatrolTarget = controller.GetCurrentPatrolPoint();
    }

    public override void OnUpdate()
    {
        var controller = context.GetController();

        if (controller.IsPlayerInChaseRange())
        {
            context.ChangeState(EnemyState.Chase);
            return;
        }

        if (currentPatrolTarget == null) return;

        controller.MoveTowards(currentPatrolTarget.position);

        float distance = Vector3.Distance(controller.transform.position, currentPatrolTarget.position);
        if (distance < 0.5f)
        {
            controller.MoveToNextPatrolPoint();
            currentPatrolTarget = controller.GetCurrentPatrolPoint();
        }
    }

    public override void OnExit() { }
}
