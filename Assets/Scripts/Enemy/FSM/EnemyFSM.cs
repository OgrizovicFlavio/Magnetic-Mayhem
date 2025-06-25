using System.Collections.Generic;

public class EnemyFSM
{
    private EnemyController controller;
    private List<BaseEnemyState> states;
    private BaseEnemyState currentState;

    public EnemyController GetController() => controller;

    public EnemyState CurrentStateType => currentState.GetStateType();

    public EnemyFSM(EnemyController controller)
    {
        this.controller = controller;

        states = new List<BaseEnemyState>
        {
            new PatrolEnemyState(this),
            new ChaseEnemyState(this),
            new AttackEnemyState(this),
            new MagnetizedEnemyState(this),
        };
    }

    public void OnUpdate()
    {
        currentState?.OnUpdate();
    }

    public void ChangeState(EnemyState newState)
    {
        if (currentState?.GetStateType() == newState)
            return;

        currentState?.OnExit();
        currentState = states.Find(s => s.GetStateType() == newState);

        currentState.OnEnter();
    }

    public BaseEnemyState FindNextState()
    {
        if (controller.IsPlayerInAttackRange())
            return states.Find(s => s.GetStateType() == EnemyState.Attack);

        if (controller.IsPlayerInChaseRange())
            return states.Find(s => s.GetStateType() == EnemyState.Chase);

        return states.Find(s => s.GetStateType() == EnemyState.Patrol);
    }
}
