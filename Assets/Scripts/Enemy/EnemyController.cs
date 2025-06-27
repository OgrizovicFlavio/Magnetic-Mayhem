using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private Transform[] patrolPoints;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float impactThreshold = 1f;
    [SerializeField] private float impactDamageMultiplier = 10f;

    private EnemyFSM fsm;
    private EnemyAttack enemyAttack;
    private EnemyMovement enemyMovement;
    private Rigidbody rb;
    private Transform target;

    private float currentHealth;
    private int magnetizeCount = 0;
    private int lastPatrolIndex = -1;
    private int currentPatrolIndex = 0;

    private void Start()
    {
        fsm.ChangeState(EnemyState.Patrol);
    }

    private void Awake()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody>();
        enemyAttack = GetComponent<EnemyAttack>();
        enemyMovement = GetComponent<EnemyMovement>();

        fsm = new EnemyFSM(this);

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            target = player.transform;

            Collider playerCollider = player.GetComponent<Collider>();
            Collider enemyCollider = GetComponent<Collider>();
            if (playerCollider != null && enemyCollider != null)
            {
                Physics.IgnoreCollision(enemyCollider, playerCollider);
            }
        }
    }

    private void Update()
    {
        fsm.OnUpdate();

        if (fsm.CurrentStateType != EnemyState.Magnetized)
        {
            Debug.DrawRay(transform.position, rb.velocity, Color.cyan, 0.1f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Utilities.CheckLayerInMask(LayerMask.GetMask("Player"), collision.gameObject.layer))
            return;

        float impactForce = collision.relativeVelocity.magnitude;

        if (impactForce >= impactThreshold)
        {
            float damage = impactForce * impactDamageMultiplier;
            TakeDamage(damage);
            Debug.Log($"[DAMAGE] {name} recibió {damage} por colisión con {collision.gameObject.name}");
        }

        if (fsm.CurrentStateType == EnemyState.Magnetized &&
            !Utilities.CheckLayerInMask(LayerMask.GetMask("Projectile"), collision.gameObject.layer))
        {
            Debug.Log($"[MAGNET] {name} colisionó mientras estaba magnetizado");
            enemyMovement.RegisterMagnetizedCollision();
        }
    }

    #region FSM

    public Transform GetTarget() => target;
    public EnemyAttack GetAttackModule() => enemyAttack;

    public bool IsPlayerInChaseRange()
    {
        if (target == null) return false;
        return Vector3.Distance(transform.position, target.position) <= chaseRange;
    }

    public bool IsPlayerInAttackRange()
    {
        if (target == null) return false;
        return Vector3.Distance(transform.position, target.position) <= attackRange;
    }

    public void AttackPlayer()
    {
        enemyAttack?.TryAttack();
    }

    #endregion

    #region Magnetism

    public void SetMagnetize(bool active)
    {
        if (active)
            magnetizeCount++;
        else
            magnetizeCount = Mathf.Max(0, magnetizeCount - 1);
    }

    public bool IsMagnetized() => magnetizeCount > 0;

    public bool HasCollidedWhileMagnetized() => enemyMovement != null && enemyMovement.UpdateHeight(0.2f);
    public void ResetCollisionFlag() => enemyMovement?.ResetMagnetizedCollision();

    public void ApplyRepulsion(Vector3 direction, float force)
    {
        enemyMovement?.ApplyRepulsion(direction, force);
        fsm.ChangeState(EnemyState.Magnetized);
        SetMagnetize(true);
    }

    public bool UpdateHeight(float threshold = 0.1f)
    {
        return enemyMovement != null && enemyMovement.UpdateHeight(threshold);
    }

    #endregion

    #region Patrol System

    public Transform GetCurrentPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return null;

        return patrolPoints[currentPatrolIndex];
    }

    public void MoveToRandomPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length < 2) return;

        int newIndex = currentPatrolIndex;
        int maxTries = 10;
        int tries = 0;

        while ((newIndex == currentPatrolIndex || Vector3.Distance(transform.position, patrolPoints[newIndex].position) < 1.5f)
               && tries < maxTries)
        {
            newIndex = Random.Range(0, patrolPoints.Length);
            tries++;
        }

        lastPatrolIndex = currentPatrolIndex;
        currentPatrolIndex = newIndex;
    }

    #endregion

    #region Life System

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"[DAMAGE] {name} took {amount} damage. Remaining: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    #endregion
}
