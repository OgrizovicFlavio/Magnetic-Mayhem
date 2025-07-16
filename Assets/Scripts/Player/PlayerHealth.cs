using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    private DamageEffect damageEffect;

    private void Start()
    {
        currentHealth = maxHealth;
        damageEffect = GetComponentInChildren<DamageEffect>();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        UIManager.Instance?.SetHealth(currentHealth, maxHealth);

        if (damageEffect != null)
            damageEffect.Flash();

        if (currentHealth <= 0)
            Die();
    }

    public float GetHealth() => currentHealth;

    private void Die()
    {
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
