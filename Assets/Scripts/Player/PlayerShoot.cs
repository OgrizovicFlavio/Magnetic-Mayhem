using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Controller controller;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform firePoint;
    [SerializeField] private PlayerAim playerAim;

    [Header("Settings")]
    [SerializeField] private float fireRate = 0.5f;

    private float nextFireTime = 0f;

    public void TryShoot()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        //Disaro basado en la dirección de la mira
        Ray ray = playerAim.GetAimingRay();
        Vector3 direction = ray.direction;

        Sticky sticky = PoolManager.Instance.Get<Sticky>(firePoint.position, Quaternion.LookRotation(direction));
        sticky.SetCharge(controller.GetCurrentCharge());
        sticky.Launch(direction);
    }
}
