using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Sticky : MonoBehaviour, IPooleable
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private LayerMask impactMask;

    private Rigidbody rb;
    private Rigidbody targetRb = null;
    private Magnet magnet;
    private Vector3 launchDirection;
    private bool hasImpacted = false;
    private bool wasKinematic = false;
    private float spawnTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        magnet = GetComponent<Magnet>();
    }

    public void Launch(Vector3 direction)
    {
        hasImpacted = false;
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
        if (hasImpacted) return;
        hasImpacted = true;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Transform hitTransform = collision.transform;
        transform.position = collision.contacts[0].point;
        transform.rotation = Quaternion.LookRotation(-collision.contacts[0].normal);

        Rigidbody hitRb = collision.rigidbody;

        if (hitRb != null && !hitRb.isKinematic)
        {
            wasKinematic = hitRb.isKinematic;
            targetRb = hitRb;

            if (!hitRb.isKinematic)
                hitRb.isKinematic = true;
        }

        rb.isKinematic = true;

        magnet.ActivateMagnet();

        Invoke(nameof(Deactivate), lifetime);
    }

    private void Deactivate()
    {
        hasImpacted = false;
        magnet.DeactivateMagnet();
        transform.SetParent(null);
        rb.isKinematic = false;

        if (targetRb != null)
        {
            targetRb.isKinematic = wasKinematic;
            targetRb = null;
            wasKinematic = false;
        }

        PoolManager.Instance.ReturnToPool(this);
    }

    public void GetObjectFromPool()
    {
        hasImpacted = false;

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
    }

    public void ReturnObjectToPool() { }

    public void ResetToDefault()
    {
        GetObjectFromPool();
        magnet.SetCharge(MagneticChargeType.None);
        magnet.DeactivateMagnet();
    }

    public void Disable()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        magnet.DeactivateMagnet();
    }

    public void SetCharge(MagneticChargeType newCharge)
    {
        magnet.SetCharge(newCharge);
    }
}
