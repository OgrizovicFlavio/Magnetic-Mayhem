using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"[PLAYER] Recibe {amount} de daño. Vida restante: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    public float GetHealth() => currentHealth;

    private void Die()
    {
        Debug.Log("[PLAYER] ¡El jugador ha muerto!");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!Utilities.CheckLayerInMask(LayerMask.GetMask("Magnetic"), collision.gameObject.layer))
            return;

        if (collision.gameObject.TryGetComponent<EnemyController>(out var enemy))
        {
            var attack = enemy.GetAttackModule();
            if (attack != null)
                TakeDamage(attack.GetDamageAmount());
        }
    }
}
