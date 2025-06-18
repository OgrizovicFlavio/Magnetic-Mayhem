using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Magnet : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Sticky stickyOwner;
    [SerializeField] private MagneticChargeType magneticCharge = MagneticChargeType.None;
    [SerializeField] private float magneticForce = 10f;
    [SerializeField] private MagnetDetector magnetDetector;
    [SerializeField] private MagnetType magnetType = MagnetType.Ferrous;

    [Header("Colors")]
    [SerializeField] private Renderer visualRenderer;
    private static readonly Color[] chargeColors = new Color[3]
    {
        Color.gray,
        Color.red,
        Color.blue
    };

    private Rigidbody rb;
    private List<Magnet> magnetsInRange = new List<Magnet>();
    private MaterialPropertyBlock block;
    private bool isMagnetActive = false;

    public MagnetType Type => magnetType;
    public MagneticChargeType Charge => magneticCharge;
    public float Force => magneticForce;
    public Rigidbody Rb => rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        block = new MaterialPropertyBlock();

        if (magnetDetector != null)
        {
            magnetDetector.onMagnetEnter += OnMagnetEnter;
            magnetDetector.onMagnetExit += OnMagnetExit;
        }

        SetColorByCharge(magneticCharge);
    }

    private void FixedUpdate()
    {
        if (!isMagnetActive || magnetDetector == null)
            return;

        foreach (var other in magnetsInRange)
        {
            if (other == null || other == this)
                continue;

            if (this.Type == MagnetType.Sticky && other.Type == MagnetType.Sticky)
                continue;

            if (Charge == MagneticChargeType.None || other.Charge == MagneticChargeType.None)
                continue;

            Vector3 direction = (other.transform.position - transform.position).normalized;

            if (Charge != other.Charge)
                direction *= -1;

            float adjustedForce = magneticForce / Mathf.Max(rb.mass, 1f);
            other.Rb.AddForce(direction * adjustedForce, ForceMode.Force);
        }
    }

    private void DetectInitialMagnets()
    {
        Collider[] colliders = Physics.OverlapSphere(magnetDetector.transform.position,
                                                      ((SphereCollider)magnetDetector.GetComponent<Collider>()).radius,
                                                      LayerMask.GetMask("Magnetic"));
        foreach (var col in colliders)
        {
            Magnet other = col.GetComponent<Magnet>();
            if (other != null && other != this && !magnetsInRange.Contains(other))
            {
                magnetsInRange.Add(other);
            }
        }
    }

    public void ActivateMagnet()
    {
        isMagnetActive = true;
        SetColorByCharge(magneticCharge);

        if (magnetDetector != null)
        {
            DetectInitialMagnets();
        }
    }

    public void DeactivateMagnet()
    {
        isMagnetActive = false;
    }

    public void SetCharge(MagneticChargeType newCharge)
    {
        magneticCharge = newCharge;

        SetColorByCharge(newCharge);
    }

    public void SetStickyOwner(Sticky sticky)
    {
        stickyOwner = sticky;
    }

    private void OnMagnetEnter(Magnet m)
    {
        if (!magnetsInRange.Contains(m))
            magnetsInRange.Add(m);

        EnemyController enemy = m.GetComponentInParent<EnemyController>();
        if (enemy != null)
        {
            enemy.Magnetize();
        }
    }

    private void OnMagnetExit(Magnet m)
    {
        if (magnetsInRange.Contains(m))
            magnetsInRange.Remove(m);

        EnemyController enemy = m.GetComponent<EnemyController>();
        if (enemy != null && stickyOwner != null && stickyOwner.IsDeactivating())
            return;

        if (enemy != null)
        {
            enemy.Unmagnetize();
        }
    }

    private void SetColorByCharge(MagneticChargeType charge)
    {
        if (visualRenderer == null) 
            return;

        block.SetColor("_Color", chargeColors[(int)charge]);
        visualRenderer.SetPropertyBlock(block);
    }

    private void OnDestroy()
    {
        if (magnetDetector != null)
        {
            magnetDetector.onMagnetEnter -= OnMagnetEnter;
            magnetDetector.onMagnetExit -= OnMagnetExit;
        }
    }
}
