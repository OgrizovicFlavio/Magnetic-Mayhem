using UnityEngine;

public class MagneticProjectile : ProjectileBase
{
    [Header("Settings")]
    [SerializeField] private Renderer visualRenderer;
    [SerializeField] private Color positiveColor = Color.red;
    [SerializeField] private Color negativeColor = Color.blue;
    [SerializeField] private LayerMask magneticLayer;
    [SerializeField] float effectRadius = 10f;
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

    protected override MagneticChargeType GetChargeType()
    {
        return charge;
    }

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

        //Punto de impacto
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, 2f, magneticLayer))
        {
            transform.position = hit.point;
            transform.rotation = Quaternion.LookRotation(-hit.normal);
        }

        //Rigidbody se vuelve kinematic
        rb.isKinematic = true;

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

        //Aplico fuerza magnética a objetos cercanos
        ApplyMagneticEffect();

        //Desactivación
        Invoke(nameof(Deactivate), 5f);
    }

    private void ApplyMagneticEffect()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, effectRadius, magneticLayer);

        foreach (Collider col in colliders)
        {
            if (col.transform == this.transform)
                continue;

            IMagnetic magneticTarget = col.GetComponent<IMagnetic>();
            if (magneticTarget != null)
            {
                magneticTarget.ApplyMagneticForce(transform.position, magneticForce, charge);
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
