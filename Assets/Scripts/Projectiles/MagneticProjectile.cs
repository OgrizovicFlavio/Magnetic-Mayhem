using UnityEngine;

public class MagneticProjectile : ProjectileBase, IMagneticEmitter
{
    [Header("Settings")]
    [SerializeField] private Renderer visualRenderer;
    [SerializeField] private Color positiveColor = Color.red;
    [SerializeField] private Color negativeColor = Color.blue;
    [SerializeField] private LayerMask magneticLayer;
    [SerializeField] float effectRadius = 20f;
    [SerializeField] float magneticForce = 10f;

    private MagneticChargeType charge;
    private Transform hitSurface;
    private bool hasImpacted = false;

    //Seteo la carga magnética y el color del proyectil
    public void SetCharge(MagneticChargeType newCharge)
    {
        charge = newCharge;

        if (visualRenderer != null)
        {
            if (charge == MagneticChargeType.Positive)
                visualRenderer.material.color = positiveColor;
            else
                visualRenderer.material.color = negativeColor;
        }
    }

    public float GetEffectRadius() => effectRadius;
    public float GetForceStrength() => magneticForce;
    public Vector3 GetPosition() => transform.position;
    public MagneticChargeType GetChargeType() => charge;

    protected override void OnTriggerEnter(Collider other)
    {
        //Evito múltiples impactos antes del tiempo mínimo
        if (Time.time - spawnTime < GetCollisionDelay())
            return;

        //Chequeo de layer
        if (!Utilities.CheckLayerInMask(magneticLayer, other.gameObject.layer))
            return;

        //Guardo su superficie
        hitSurface = other.transform;

        //Lógica de impacto
        OnImpact();
    }

    protected override void OnImpact()
    {
        if (hasImpacted) return;
        hasImpacted = true;

        //Freno el movimiento
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //Rigidbody se vuelve kinematic
        rb.isKinematic = true;

        //Detecto la superficie de impacto
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, 2f, magneticLayer))
        {
            transform.position = hit.point;
            transform.rotation = Quaternion.LookRotation(-hit.normal);
        }

        // Cambiar la carga del objeto alcanzado si tiene MagneticObject
        MagneticObject magneticTarget = hitSurface.GetComponent<MagneticObject>();
        if (magneticTarget != null)
        {
            magneticTarget.SetCharge(charge);
        }

        //Si el objeto impactado tiene Rigidbody, se une con un FixedJoint
        Rigidbody targetRb = hitSurface.GetComponent<Rigidbody>();
        if (targetRb != null)
        {
            FixedJoint joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = targetRb;
            joint.enablePreprocessing = false;
            joint.breakForce = Mathf.Infinity;
            joint.breakTorque = Mathf.Infinity;
        }

        //Desactivación
        Invoke(nameof(Deactivate), 5f);
    }

    private void FixedUpdate()
    {
        if (!hasImpacted)
            return;

        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.SphereCast(ray, 0.5f, out RaycastHit hit, effectRadius, magneticLayer))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.yellow);
        }

        ApplyMagneticEffect();
    }

    private void ApplyMagneticEffect()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, effectRadius, magneticLayer);

        foreach (Collider col in colliders)
        {
            if (col.transform == this.transform) 
                continue;

            IMagneticReceiver receiver = col.GetComponent<IMagneticReceiver>();
            if (receiver != null)
            {
                receiver.ApplyMagneticForce(transform.position, magneticForce, charge);
            }
        }
    }

    private void Deactivate()
    {
        //Elimino el FixedJoint antes de regresar a la pool
        FixedJoint joint = GetComponent<FixedJoint>();
        if (joint != null)
            Destroy(joint);

        PoolManager.Instance.ReturnToPool(this);
    }

    public override void GetObjectFromPool()
    {
        hasImpacted = false;
        charge = MagneticChargeType.Positive;
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public override void ReturnObjectToPool() { }

    public override void ResetToDefault()
    {
        hasImpacted = false;
        charge = MagneticChargeType.Positive;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        FixedJoint joint = GetComponent<FixedJoint>();
        if (joint != null)
            Destroy(joint);
    }

    public override void Disable()
    {
        if (!rb.isKinematic)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        FixedJoint joint = GetComponent<FixedJoint>();
        if (joint != null)
            Destroy(joint);
    }


}
