using UnityEngine;

public class MagnetizedEnemyState : BaseEnemyState
{
    public MagnetizedEnemyState(EnemyFSM fsm) : base(fsm)
    {
        enemyState = EnemyState.Magnetized;
    }

    public override void OnEnter()
    {
        controller.SetUnderMagnetEffect(true);
        movement.StopMovement();
        controller.GetRigidbody().velocity = Vector3.zero;
    }

    public override void OnUpdate()
    {
        if (!controller.CanResumeFSM())
            return;

        // Recuperar altura
        float currentY = controller.transform.position.y;
        float targetY = controller.GetInitialHeight();
        float deltaY = Mathf.Abs(currentY - targetY);
        float recoverSpeed = controller.WasRepelled() ? 2f : 6f;

        // Siempre que la altura no sea la original, la forzamos
        if (deltaY > 0.05f)
        {
            Vector3 pos = controller.transform.position;
            pos.y = Mathf.MoveTowards(pos.y, targetY, Time.deltaTime * recoverSpeed);
            controller.transform.position = pos;
            return;
        }

        // Ya puede salir del estado
        controller.SetWasRepelled(false);
        controller.SetUnderMagnetEffect(false);
        controller.ResetCollisionFlag();
        controller.SetMagnetize(false);

        var next = context.FindNextState().GetStateType();
        context.ChangeState(next);
    }

    public override void OnExit()
    {
        movement.StopMovement();
        controller.SetUnderMagnetEffect(false);
        controller.ResetCollisionFlag();
        controller.SetMagnetize(false);
    }
}