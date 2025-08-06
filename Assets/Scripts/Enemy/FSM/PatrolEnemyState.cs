using UnityEngine;

public class PatrolEnemyState : BaseEnemyState
{
    private Transform currentTarget;

    public PatrolEnemyState(EnemyFSM fsm) : base(fsm)
    {
        enemyState = EnemyState.Patrol;
    }

    public override void OnEnter()
    {
        movement.StopMovement();

        // Si fue repelido, usar el punto más cercano
        controller.SetClosestPatrolPointAsCurrent();
        currentTarget = controller.GetCurrentPatrolPoint();
    }

    public override void OnUpdate()
    {
        if (controller.IsMagnetized())
        {
            context.ChangeState(EnemyState.Magnetized);
            return;
        }

        if (controller.IsPlayerInChaseRange())
        {
            context.ChangeState(EnemyState.Chase);
            return;
        }

        if (currentTarget == null) return;

        float distance = Vector3.Distance(controller.transform.position, currentTarget.position);

        if (distance < 0.5f)
        {
            controller.GoToNextPatrolPoint();
            currentTarget = controller.GetCurrentPatrolPoint();
        }

        float height = controller.GetInitialHeight();
        movement.MoveTowards(currentTarget.position, height);
        movement.RotateTowards(currentTarget.position);
        Debug.DrawLine(controller.transform.position, currentTarget.position, Color.green, 0.1f);
    }

    public override void OnExit()
    {
        movement.StopMovement();
    }
}
