public abstract class BaseEnemyState : BaseState<EnemyFSM>
{
    protected EnemyState enemyState;

    public EnemyState GetStateType() => enemyState;

    public BaseEnemyState(EnemyFSM fsm) : base(fsm) { }
}
