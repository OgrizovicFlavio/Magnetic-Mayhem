using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Projectile Prefabs")]
    [SerializeField] private MagneticProjectile magneticProjectilePrefab;

    private void Start()
    {
        PoolManager.Instance.InitializePool(magneticProjectilePrefab, 20);
    }
}
