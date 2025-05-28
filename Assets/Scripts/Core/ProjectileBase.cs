using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour, IPooleable
{
    [SerializeField] protected float speed;
    [SerializeField] protected float damage;
    [SerializeField] protected Rigidbody rb;

    protected Vector3 direction;
    protected float spawnTime;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public virtual void Launch(Vector3 dir)
    {
        direction = dir.normalized; //Dirección 
        rb.velocity = direction * speed; //Velocidad
        spawnTime = Time.time;
    }

    protected abstract MagneticChargeType GetChargeType();
    protected abstract void OnImpact(); 

    protected virtual float GetCollisionDelay() => 0.05f; 

    protected abstract void OnTriggerEnter(Collider other);

    public abstract void GetObjectFromPool();
    public abstract void ReturnObjectToPool();
    public abstract void ResetToDefault();
    public abstract void Disable();
}
