using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMotor motor;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject laserSightPrefab;

    [Header("Laser Settings")]
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private Color positiveColor = Color.red;
    [SerializeField] private Color negativeColor = Color.blue;

    private LineRenderer lineRenderer;

    private void Start()
    {
        GameObject laser = Instantiate(laserSightPrefab, transform);
        lineRenderer = laser.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        UpdateLaser();
    }

    private void UpdateLaser()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 direction = ray.direction;
        Vector3 start = firePoint.position;
        Vector3 end = start + direction * maxDistance;

        if (Physics.Raycast(start, direction, out RaycastHit hit, maxDistance))
        {
            end = hit.point;
        }

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        MagneticChargeType charge = motor.GetCurrentCharge();
        if (charge == MagneticChargeType.Positive)
        {
            lineRenderer.startColor = positiveColor;
            lineRenderer.endColor = positiveColor;
        }
        else
        {
            lineRenderer.startColor = negativeColor;
            lineRenderer.endColor = negativeColor;
        }
    }

    public Ray GetAimingRay()
    {
        return playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    }
}
