using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMotor motor;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform firePoint;
    [SerializeField] private PlayerAim playerAim;

    [Header("Settings")]
    [SerializeField] private float fireRate = 0.5f;

    private float nextFireTime = 0f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        if (Input.GetMouseButtonDown(1))
        {
            motor.ToggleCharge();
        }
    }

    private void Shoot()
    {
        //Disaro basado en la dirección de la mira
        Ray ray = playerAim.GetAimingRay();
        Vector3 direction = ray.direction;

        //Instancio desde la pool
        MagneticProjectile projectile = PoolManager.Instance.Get<MagneticProjectile>(firePoint.position, Quaternion.LookRotation(direction));

        projectile.SetCharge(motor.GetCurrentCharge()); //Asigna la carga actual
        projectile.Launch(direction); //Disparo
    }
}
