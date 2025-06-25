public abstract class BaseEnemyState
{
    protected EnemyFSM context;
    protected EnemyController controller;
    protected EnemyMovement movement;
    protected EnemyAttack attack;
    protected EnemyState enemyState;

    public BaseEnemyState(EnemyFSM context)
    {
        this.context = context;
        this.controller = context.GetController();
        this.movement = controller.GetComponent<EnemyMovement>();
        this.attack = controller.GetComponent<EnemyAttack>();
    }

    public EnemyState GetStateType() => enemyState;

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
}
