using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Sticky : MonoBehaviour, IPooleable
{
    [Header("Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float collisionDelay = 0.05f;

    private Rigidbody rb;
    private Magnet magnet;
    private FixedJoint fixedJoint;
    private Vector3 launchDirection;
    private bool isDeactivating = false;
    private bool hasImpacted = false;
    private float spawnTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        magnet = GetComponent<Magnet>();
    }

    public void Launch(Vector3 direction)
    {
        hasImpacted = false;
        spawnTime = Time.time;

        launchDirection = direction.normalized;
        rb.velocity = launchDirection * speed;
    }

    private void Update()
    {
        if (hasImpacted) return;

        if (Time.time - spawnTime > lifetime)
        {
            Deactivate();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasImpacted) 
            return;

        if (Time.time - spawnTime < collisionDelay && LayerMask.NameToLayer("Ground") != collision.gameObject.layer)
            return;

        hasImpacted = true;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        Transform hitTransform = collision.transform;
        transform.position = collision.contacts[0].point;

        Rigidbody otherRb = hitTransform.GetComponentInParent<Rigidbody>();

        if (otherRb != null && otherRb != rb)
        {
            fixedJoint = gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = otherRb;
            fixedJoint.enableCollision = false;
        }
        else
        {
            // Adherir al transform si no tiene Rigidbody
            transform.SetParent(hitTransform);
        }

        magnet?.ActivateMagnet();

        Magnet otherMagnet = hitTransform.GetComponent<Magnet>();
        if (otherMagnet != null)
        {
            magnet.IgnoreMagnet(otherMagnet);
        }

        // Se auto-desactiva después del tiempo de vida
        Invoke(nameof(Deactivate), lifetime);
    }

    public void Deactivate()
    {
        if (isDeactivating) return;
        isDeactivating = true;

        magnet?.DeactivateMagnet();
        transform.SetParent(null);

        if (fixedJoint != null)
            Destroy(fixedJoint);

        rb.isKinematic = false;
        rb.velocity = Vector3.zero;

        PoolManager.Instance.ReturnToPool(this);
    }

    public void GetObjectFromPool()
    {
        gameObject.SetActive(true);
    }

    public void ReturnObjectToPool()
    {
        gameObject.SetActive(false);
    }

    public void ResetToDefault()
    {
        hasImpacted = false;
        isDeactivating = false;

        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.SetParent(null);

        if (fixedJoint != null)
            Destroy(fixedJoint);

        magnet?.DeactivateMagnet();
        magnet?.RemoveAllMagnets();
    }

    public void Disable()
    {
        CancelInvoke();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false;
        transform.SetParent(null);

        if (fixedJoint != null)
            Destroy(fixedJoint);

        gameObject.SetActive(false);
    }

    internal void SetCharge(MagneticChargeType currentCharge)
    {
        if (magnet != null)
            magnet.SetCharge(currentCharge);
    }
}
