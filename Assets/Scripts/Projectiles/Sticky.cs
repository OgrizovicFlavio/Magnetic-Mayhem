using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Sticky : MonoBehaviour, IPooleable
{
    [Header("Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float collisionDelay = 0.05f;

    private Rigidbody rb;
    private Rigidbody targetRb = null;
    private Magnet magnet;
    private Vector3 launchDirection;
    private bool isDeactivating = false;
    private bool hasImpacted = false;
    private bool wasKinematic = false;
    private float spawnTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        magnet = GetComponent<Magnet>();

        if (magnet != null)
            magnet.SetStickyOwner(this);
    }

    public void Launch(Vector3 direction)
    {
        hasImpacted = false;
        isDeactivating = false;
        launchDirection = direction.normalized;
        rb.velocity = launchDirection * speed;
        spawnTime = Time.time;
    }

    private void Update()
    {
        if (!hasImpacted && Time.time - spawnTime > lifetime)
        {
            Deactivate();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasImpacted || Time.time - spawnTime < collisionDelay)
            return;

        hasImpacted = true;

        if (!rb.isKinematic)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Ver si es enemigo
        bool isEnemy = collision.transform.TryGetComponent<EnemyController>(out _);

        // Pegarse visualmente
        if (isEnemy)
        {
            transform.SetParent(collision.transform);
        }
        else
        {
            transform.position = collision.contacts[0].point;
            transform.SetParent(collision.transform);
            transform.rotation = Quaternion.LookRotation(-collision.contacts[0].normal);
        }

        // Si tiene Rigidbody y NO es enemigo, lo inmovilizamos
        targetRb = collision.rigidbody;
        if (targetRb != null && !isEnemy)
        {
            wasKinematic = targetRb.isKinematic;
            if (!targetRb.isKinematic)
                targetRb.isKinematic = true;
        }

        rb.isKinematic = true;

        if (magnet != null)
            magnet.ActivateMagnet();

        Invoke(nameof(Deactivate), lifetime);
    }

    private void Deactivate()
    {
        isDeactivating = true;
        hasImpacted = false;

        if (magnet != null)
            magnet.DeactivateMagnet();

        if (targetRb != null)
        {
            targetRb.isKinematic = wasKinematic;
            targetRb = null;
            wasKinematic = false;
        }

        transform.SetParent(null);
        rb.isKinematic = false;

        PoolManager.Instance.ReturnToPool(this);
    }

    public bool IsDeactivating()
    {
        return isDeactivating;
    }

    public void SetCharge(MagneticChargeType newCharge)
    {
        if (magnet != null)
            magnet.SetCharge(newCharge);
    }

    public void GetObjectFromPool()
    {
        hasImpacted = false;
        isDeactivating = false;

        if (targetRb != null)
        {
            targetRb.isKinematic = wasKinematic;
            targetRb = null;
            wasKinematic = false;
        }

        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.SetParent(null);

        if (magnet != null)
        {
            magnet.SetCharge(MagneticChargeType.None);
            magnet.DeactivateMagnet();
        }
    }

    public void ReturnObjectToPool() { }

    public void ResetToDefault()
    {
        GetObjectFromPool();
    }

    public void Disable()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        if (magnet != null)
            magnet.DeactivateMagnet();
    }
}
