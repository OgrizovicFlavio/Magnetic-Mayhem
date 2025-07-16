using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackForce = 20f;
    [SerializeField] private float retreatForceMultiplier = 0.5f;
    [SerializeField] private float retreatDuration = 1.5f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackRange = 5f;

    private bool canAttack = true;
    private bool isRetreating = false;

    private Transform player;
    private Rigidbody rb;
    private EnemyMovement enemyMovement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enemyMovement = GetComponent<EnemyMovement>();
    }

    public void SetPlayer(Transform player)
    {
        this.player = player;
    }

    public void TryAttack()
    {
        if (!canAttack || isRetreating || player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > attackRange)
            return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        canAttack = false;

        enemyMovement?.StopMovement();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 dir = (player.position - transform.position);
        dir.y = 0f;
        dir.Normalize();

        float chargeTime = 0.3f;
        float elapsed = 0f;
        float chargeSpeed = attackForce;

        while (elapsed < chargeTime)
        {
            rb.MovePosition(rb.position + dir * chargeSpeed * Time.fixedDeltaTime);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

 
        yield return new WaitForSeconds(0.1f);

        isRetreating = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 retreatDir = -dir;
        rb.AddForce(retreatDir * (attackForce * retreatForceMultiplier), ForceMode.Impulse);

        yield return new WaitForSeconds(retreatDuration);

        isRetreating = false;
        canAttack = true;
    }

    public bool IsRetreating() => isRetreating;
    public float GetDamageAmount() => damage;
}
