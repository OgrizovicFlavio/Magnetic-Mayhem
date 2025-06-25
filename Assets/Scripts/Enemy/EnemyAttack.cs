using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackForce = 10f;
    [SerializeField] private float retreatDuration = 0.75f;
    [SerializeField] private float damage = 25f;
    [SerializeField] private float attackRange = 3f;

    private bool canAttack = true;
    private bool isRetreating = false;

    private Transform player;
    private EnemyMovement enemyMovement;

    private void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    #region Attack Logic

    public void TryAttack()
    {
        if (!canAttack || isRetreating || player == null)
            return;

        if (Vector3.Distance(transform.position, player.position) > attackRange)
            return;

        if (player.TryGetComponent<PlayerHealth>(out var health))
        {
            health.TakeDamage(damage);
            Debug.Log("[ENEMY ATTACK] Jugador dañado por " + damage);
        }

        StartCoroutine(RetreatThenCooldown());
    }

    #endregion

    #region Retreat

    private IEnumerator RetreatThenCooldown()
    {
        canAttack = false;
        isRetreating = true;

        enemyMovement?.RetreatFrom(player.position, attackForce, retreatDuration);

        yield return new WaitForSeconds(retreatDuration);

        isRetreating = false;
        canAttack = true;
    }

    public bool IsRetreating() => isRetreating;

    #endregion

    public float GetDamageAmount() => damage;
}
