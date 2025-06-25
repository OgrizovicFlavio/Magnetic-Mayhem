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

        movement.StopMovement();

        controller.MoveToRandomPatrolPoint();
        currentPatrolTarget = controller.GetCurrentPatrolPoint();
    }

    public override void OnUpdate()
    {
        if (controller.IsPlayerInChaseRange())
        {
            context.ChangeState(EnemyState.Chase);
            return;
        }

        if (controller.IsMagnetized())
        {
            context.ChangeState(EnemyState.Magnetized);
            return;
        }

        if (currentPatrolTarget == null)
            return;

        float distance = Vector3.Distance(controller.transform.position, currentPatrolTarget.position);

        if (distance < 0.5f)
        {
            controller.MoveToRandomPatrolPoint();
            currentPatrolTarget = controller.GetCurrentPatrolPoint();
            return;
        }

        movement.MoveTowards(currentPatrolTarget.position);
        movement.RotateTowards(currentPatrolTarget.position);
        Debug.DrawLine(controller.transform.position, currentPatrolTarget.position, Color.green, 0.1f);
    }

    public override void OnExit() { }
}
