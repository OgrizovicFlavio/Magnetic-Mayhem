using UnityEngine;

public class MagnetizedEnemyState : BaseEnemyState
{
    private EnemyState previousState;

    public EnemyState PreviousState => previousState;

    public MagnetizedEnemyState(EnemyFSM fsm) : base(fsm)
    {
        enemyState = EnemyState.Magnetized;
    }

    public void Set(EnemyState previous)
    {
        this.previousState = previous;
    }

    public override void OnEnter() { }

    public override void OnUpdate()
    {

    }

    public override void OnExit() { }
}
