using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform firePoint;
    [SerializeField] private LineRenderer laserRenderer;
    [SerializeField] private LayerMask raycastMask;

    [Header("Settings")]
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float maxDistance = 100f;

    [Header("Charge")]
    private MagneticChargeType currentCharge = MagneticChargeType.Positive;

    private Transform cameraTransform;
    private PlayerController controller;

    private float nextFireTime = 0f;

    public void Initialize(Transform cameraTransform, PlayerController controller)
    {
        this.cameraTransform = cameraTransform;
        this.controller = controller;
    }

    private void Update()
    {
        UpdateLaser();
    }

    private void OnEnable()
    {
        if (laserRenderer != null)
            laserRenderer.enabled = true;
    }

    private void OnDisable()
    {
        if (laserRenderer != null)
            laserRenderer.enabled = false;
    }

    private void UpdateLaser()
    {
        if (laserRenderer == null || firePoint == null || cameraTransform == null) 
            return;

        Ray ray = new Ray(firePoint.position, cameraTransform.forward);
        Vector3 end = ray.origin + ray.direction * maxDistance;

        if (Physics.Raycast(ray, out var hit, maxDistance, raycastMask))
            end = hit.point;

        laserRenderer.positionCount = 2;
        laserRenderer.SetPosition(0, firePoint.position);
        laserRenderer.SetPosition(1, end);

        if (controller != null)
        {
            Color color = currentCharge == MagneticChargeType.Positive ? Color.red : Color.blue;
            laserRenderer.startColor = color;
            laserRenderer.endColor = color;
        }
    }

    public void Shoot()
    {
        if (Time.time < nextFireTime) 
            return;

        nextFireTime = Time.time + fireRate;

        if (firePoint == null || cameraTransform == null) 
            return;

        Ray ray = new Ray(firePoint.position, cameraTransform.forward);
        Vector3 dir = ray.direction;

        Sticky sticky = PoolManager.Instance.Get<Sticky>(firePoint.position, Quaternion.LookRotation(dir));
        if (sticky != null)
        {
            sticky.SetCharge(currentCharge);
            sticky.Launch(dir);

            Collider stickyCollider = sticky.GetComponent<Collider>();
            Collider playerCollider = controller.GetComponentInParent<Collider>();

            if (stickyCollider != null && playerCollider != null)
                Physics.IgnoreCollision(stickyCollider, playerCollider, true);
        }
    }

    public void ToggleCharge()
    {
        currentCharge = currentCharge == MagneticChargeType.Positive ? MagneticChargeType.Negative : MagneticChargeType.Positive;
    }

    public MagneticChargeType GetCurrentCharge() => currentCharge;
}