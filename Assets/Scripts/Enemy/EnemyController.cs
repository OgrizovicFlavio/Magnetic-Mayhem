using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    private EnemyFSM fsm;

    [Header("Settings")]
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform[] patrolPoints;

    private Rigidbody rb;
    private Transform target;
    private int currentPatrolIndex = 0;
    private bool isMagnetized = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        fsm = new EnemyFSM(this);

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            target = player.transform;
    }

    private void Update()
    {
        fsm.OnUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (fsm.CurrentStateType != EnemyState.Magnetized)
            return;

        if (Utilities.CheckLayerInMask(LayerMask.GetMask("Projectile"), collision.gameObject.layer))
            return;

        Destroy(gameObject);
    }

    public void Magnetize()
    {
        if (!isMagnetized)
        {
            isMagnetized = true;
            fsm.EnterMagnetizedState();
        }
    }

    public void Unmagnetize()
    {
        if (isMagnetized)
        {
            isMagnetized = false;
            fsm.ExitMagnetizedState();
        }
    }

    public bool IsMagnetized() => isMagnetized;
    public Transform GetTarget() => target;
    public float GetChaseRange() => chaseRange;
    public float GetAttackRange() => attackRange;
    public float GetMoveSpeed() => moveSpeed;
    public Rigidbody GetRigidbody() => rb;

    public void MoveTowards(Vector3 targetPosition)
    {
        if (isMagnetized) return;

        Vector3 direction = (targetPosition - transform.position).normalized;
        rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
    }

    public void RotateTowardsTarget()
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f;
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    public Transform GetCurrentPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return null;
        return patrolPoints[currentPatrolIndex];
    }

    public void MoveToNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    public void AttackPlayer()
    {
        Debug.Log("Enemy attacks player!");
    }

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
}
