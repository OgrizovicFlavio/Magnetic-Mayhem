public abstract class BaseState<T>
{
    protected T context;

    public BaseState(T context)
    {
        this.context = context;
    }

    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnExit() { }
}
