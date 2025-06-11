public abstract class EnemyBase : CharacterBase, IEnemy, IPooleable
{
    public virtual void Init()
    {
        currentHealth = maxHealth;
    }

    public abstract void Act();
    public abstract void OnDeath();

    protected override void Die()
    {
        OnDeath();
        ReturnObjectToPool();
    }

    public abstract void GetObjectFromPool();
    public abstract void ReturnObjectToPool();
    public abstract void ResetToDefault();
    public abstract void Disable();
}
