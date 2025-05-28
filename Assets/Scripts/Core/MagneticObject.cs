using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MagneticObject : MonoBehaviour, IMagnetic
{
    [SerializeField] private Renderer visualRenderer;
    [SerializeField] private Color positiveColor = Color.red;
    [SerializeField] private Color negativeColor = Color.blue;
    [SerializeField] private MagneticChargeType initialCharge = MagneticChargeType.Positive;

    private Rigidbody rb;
    private MagneticChargeType currentCharge;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        SetCharge(initialCharge);
    }

    public void SetCharge(MagneticChargeType charge)
    {
        currentCharge = charge;

        if (visualRenderer != null)
        {
            if (charge == MagneticChargeType.Positive)
                visualRenderer.material.color = positiveColor;
            else
                visualRenderer.material.color = negativeColor;
        }
    }

    public void ApplyMagneticForce(Vector3 origin, float force, MagneticChargeType sourceCharge)
    {
        Vector3 direction = (transform.position - origin).normalized;

        // Si las cargas son opuestas, invertimos la dirección para atraer
        if (sourceCharge != currentCharge)
        {
            direction = -direction;
        }

        float adjustedForce = force / Mathf.Max(rb.mass, 1f);
        rb.AddForce(direction * adjustedForce, ForceMode.Impulse);
    }

    public MagneticChargeType GetChargeType()
    {
        return currentCharge;
    }
}

