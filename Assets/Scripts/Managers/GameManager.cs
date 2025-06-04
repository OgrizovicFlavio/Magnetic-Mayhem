using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Projectile Prefabs")]
    [SerializeField] private Sticky stickyMagnetProjectile;

    private void Start()
    {
        PoolManager.Instance.InitializePool(stickyMagnetProjectile, 20);
    }
}
