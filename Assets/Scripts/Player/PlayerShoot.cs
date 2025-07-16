using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CrosshairController crosshairController;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform firePoint;
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

    public void Shoot()
    {
        if (Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + fireRate;

        if (firePoint == null || playerCamera == null)
            return;

        // Raycast desde el centro de la pantalla (crosshair)
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint = ray.origin + ray.direction * maxDistance;

        // Si golpea algo, el punto de impacto es el target
        if (Physics.Raycast(ray, out var hit, maxDistance, raycastMask))
        {
            targetPoint = hit.point;
        }

        //Calculo la dirección desde el firePoint hacia ese punto
        Vector3 dir = (targetPoint - firePoint.position).normalized;

        Sticky sticky = PoolManager.Instance.Get<Sticky>(firePoint.position, Quaternion.LookRotation(dir));
        if (sticky != null)
        {
            sticky.SetCharge(currentCharge);
            sticky.Launch(dir);

            //Evitar colisión con el jugador
            Collider stickyCollider = sticky.GetComponent<Collider>();
            Collider playerCollider = controller.GetComponentInParent<Collider>();

            if (stickyCollider != null && playerCollider != null)
                Physics.IgnoreCollision(stickyCollider, playerCollider, true);
        }
    }

    public bool CanShootNow()
    {
        return Time.time >= nextFireTime;
    }

    public void ToggleCharge()
    {
        if (currentCharge == MagneticChargeType.Positive)
            currentCharge = MagneticChargeType.Negative;
        else
            currentCharge = MagneticChargeType.Positive;

        if (crosshairController != null)
            crosshairController.SetCharge(currentCharge);
    }
}