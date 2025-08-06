using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Magnet : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private MagneticChargeType magneticCharge = MagneticChargeType.None;
    [SerializeField] private float magneticForce = 10f;
    [SerializeField] private float repulsionForce = 10f;
    [SerializeField] private MagnetDetector magnetDetector;
    [SerializeField] private MagnetType magnetType = MagnetType.Ferrous;

    [Header("Visual Materials")]
    [SerializeField] private Material redMetalMaterial;
    [SerializeField] private Material blueMetalMaterial;
    [SerializeField] private Material neutralMetalMaterial;

    [Header("Colors")]
    [SerializeField] private Renderer visualRenderer;

    private static readonly Color[] chargeColors = new Color[3]
    {
        Color.gray,
        Color.red,
        Color.blue
    };

    private Rigidbody rb;
    private List<EnemyController> enemiesInField = new List<EnemyController>();
    private List<EnemyController> enemiesRepelledOnce = new List<EnemyController>();
    private List<Magnet> magnetsInRange = new List<Magnet>();
    private List<Magnet> magnetsIgnored = new List<Magnet>();
    private MaterialPropertyBlock block;
    private bool isMagnetActive = false;
    private float activationTime = -1f;

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

    private void OnDestroy()
    {
        if (magnetDetector != null)
        {
            magnetDetector.onMagnetEnter -= OnMagnetEnter;
            magnetDetector.onMagnetExit -= OnMagnetExit;
        }
    }

    private void FixedUpdate()
    {
        if (!isMagnetActive) return;

        foreach (var other in magnetsInRange)
        {
            if (other == null || other == this || magnetsIgnored.Contains(other))
                continue;

            if (this.Type == MagnetType.Sticky && other.Type == MagnetType.Sticky)
                continue;

            if (this.Charge == MagneticChargeType.None || other.Charge == MagneticChargeType.None)
                continue;

            var enemy = other.GetComponentInParent<EnemyController>();
            bool isSameCharge = (this.Charge == other.Charge);
            bool isOppositeCharge = (this.Charge != other.Charge);

            if (enemy != null)
            {
                if (isSameCharge)
                {
                    if (!enemiesRepelledOnce.Contains(enemy))
                    {
                        Vector3 repulsionDir = CalculateRepulsionDirection(other.transform.position);
                        float force = CalculateRepulsionForce(other.transform.position);

                        enemy.ApplyRepulsion(repulsionDir, force);
                        enemiesRepelledOnce.Add(enemy);
                    }
                    continue;
                }
                else if (isOppositeCharge)
                {
                    enemy.SetMagnetize(true, attracted: true);
                }
            }

            //Fuerza general (atracción o repulsión entre imanes no enemigos)
            Vector3 direction = (other.transform.position - transform.position).normalized;
            if (isOppositeCharge)
                direction *= -1;

            float adjustedForce = magneticForce / Mathf.Max(rb.mass, 1f);
            other.Rb.AddForce(direction * adjustedForce, ForceMode.Force);
        }
    }

    public void ActivateMagnet()
    {
        isMagnetActive = true;
        activationTime = Time.time;
        DetectInitialMagnets();
    }

    public void DeactivateMagnet()
    {
        isMagnetActive = false;

        foreach (var enemy in enemiesInField)
        {
            if (enemy != null)
                enemy.SetMagnetize(false);
        }

        enemiesInField.Clear();
        enemiesRepelledOnce.Clear();
    }

    public void SetCharge(MagneticChargeType newCharge)
    {
        magneticCharge = newCharge;
        SetColorByCharge(newCharge);
    }

    private void SetColorByCharge(MagneticChargeType charge)
    {
        if (visualRenderer == null)
            return;

        Material selectedMaterial = neutralMetalMaterial;

        if (charge == MagneticChargeType.Positive)
            selectedMaterial = redMetalMaterial;
        else if (charge == MagneticChargeType.Negative)
            selectedMaterial = blueMetalMaterial;

        visualRenderer.material = selectedMaterial;
    }

    private void DetectInitialMagnets()
    {
        Collider[] colliders = Physics.OverlapSphere(magnetDetector.transform.position,
            ((SphereCollider)magnetDetector.GetComponent<Collider>()).radius,
            LayerMask.GetMask("Magnetic"));

        foreach (var col in colliders)
        {
            var other = col.GetComponent<Magnet>();
            if (other != null && other != this && !magnetsInRange.Contains(other))
                magnetsInRange.Add(other);
        }
    }

    private void OnMagnetEnter(Magnet m)
    {
        if (!magnetsInRange.Contains(m))
            magnetsInRange.Add(m);

        var enemy = m.GetComponentInParent<EnemyController>();
        if (enemy != null && !enemiesInField.Contains(enemy))
        {
            enemiesInField.Add(enemy);
        }
    }

    private void OnMagnetExit(Magnet m)
    {
        if (Time.time - activationTime < 0.1f)
            return;

        if (magnetsInRange.Contains(m))
            magnetsInRange.Remove(m);

        var enemy = m.GetComponentInParent<EnemyController>();
        if (enemy != null && enemiesInField.Contains(enemy))
        {
            enemiesInField.Remove(enemy);

            if (!enemy.WasRepelled())
            {
                enemy.SetMagnetize(false);
            }
        }
    }

    public void IgnoreMagnet(Magnet other)
    {
        if (!magnetsIgnored.Contains(other))
            magnetsIgnored.Add(other);
    }

    public void RemoveIgnoredMagnet(Magnet other)
    {
        if (magnetsIgnored.Contains(other))
            magnetsIgnored.Remove(other);
    }

    public void RemoveAllMagnets()
    {
        foreach (var item in magnetsIgnored)
        {
            item.RemoveIgnoredMagnet(this);
        }

        magnetsIgnored.Clear();
    }

    private Vector3 CalculateRepulsionDirection(Vector3 otherPosition)
    {
        Vector3 direction = (otherPosition - transform.position);
        direction.y = 0f;
        return (direction + Vector3.up * 1.5f).normalized;
    }

    private float CalculateRepulsionForce(Vector3 otherPosition)
    {
        float distance = Mathf.Max(Vector3.Distance(transform.position, otherPosition), 0.1f);
        float attenuation = 1f / Mathf.Pow(distance, 1.2f);
        return repulsionForce * attenuation * 100f;
    }
}
