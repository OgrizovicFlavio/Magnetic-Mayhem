using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MagneticObject : MonoBehaviour, IMagneticReceiver
{
    [SerializeField] private Renderer visualRenderer;
    [SerializeField] private Color noneColor = Color.gray;
    [SerializeField] private Color positiveColor = Color.red;
    [SerializeField] private Color negativeColor = Color.blue;
    [SerializeField] private MagneticChargeType initialCharge = MagneticChargeType.None;

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
            switch (charge)
            {
                case MagneticChargeType.Positive:
                    visualRenderer.material.color = positiveColor;
                    break;
                case MagneticChargeType.Negative:
                    visualRenderer.material.color = negativeColor;
                    break;
                default:
                    visualRenderer.material.color = noneColor;
                    break;
            }
        }
    }

    public void ApplyMagneticForce(Vector3 origin, float force, MagneticChargeType sourceCharge)
    {
        if (currentCharge == MagneticChargeType.None)
            return;

        Vector3 direction = (transform.position - origin).normalized;

        //Si las cargas son opuestas, atraer
        if (sourceCharge != currentCharge)
        {
            direction = -direction;
        }

        float adjustedForce = force / Mathf.Max(rb.mass, 1f);
        rb.AddForce(direction * adjustedForce, ForceMode.Force); //Continua
    }

    public MagneticChargeType GetChargeType()
    {
        return currentCharge;
    }
}

