using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private Transform[] patrolPoints;

    private EnemyFSM fsm;
    private EnemyAttack enemyAttack;
    private EnemyMovement enemyMovement;
    private Rigidbody rb;
    private Transform target;

    private bool isAttracted = false;
    private bool isRepelled = false;
    private float magnetizedStartTime;
    private int magnetizeCount = 0;
    private bool isUnderMagnetEffect = false;
    private bool wasRepelledOnce = false;
    private bool hasCollidedWhileMagnetized = false;
    private float initialHeight;
    private int currentPatrolIndex = 0;

    #region Properties

    public bool IsMagnetized() => magnetizeCount > 0;
    public bool WasRepelled() => wasRepelledOnce;
    public void SetWasRepelled(bool value) => wasRepelledOnce = value;
    public void SetUnderMagnetEffect(bool value) => isUnderMagnetEffect = value;
    public Rigidbody GetRigidbody() => rb;
    public float GetInitialHeight() => initialHeight;
    public void ResetCollisionFlag() => hasCollidedWhileMagnetized = false;
    public bool IsAttracted() => isAttracted;
    public bool IsRepelled() => isRepelled;

    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enemyAttack = GetComponent<EnemyAttack>();
        enemyMovement = GetComponent<EnemyMovement>();
        fsm = new EnemyFSM(this);
        initialHeight = transform.position.y;

        GameObject playerObj = GameManager.Instance.GetPlayer();
        if (playerObj != null)
        {
            target = playerObj.transform;
            enemyAttack?.SetPlayer(target);
        }
    }

    private void Start()
    {
        fsm.ChangeState(EnemyState.Patrol);
    }

    private void Update()
    {
        fsm.OnUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Utilities.CheckLayerInMask(LayerMask.GetMask("Player"), collision.gameObject.layer))
            return;

        if (fsm.CurrentStateType == EnemyState.Magnetized &&
            !Utilities.CheckLayerInMask(LayerMask.GetMask("Projectile"), collision.gameObject.layer))
        {
            hasCollidedWhileMagnetized = true;
        }
    }

    public void ApplyRepulsion(Vector3 direction, float force)
    {
        if (isRepelled) return;

        StopAllMovement();
        rb.AddForce(direction.normalized * force, ForceMode.Impulse);

        SetRepelled();
        fsm.ChangeState(EnemyState.Magnetized);
    }

    #region FSM Access

    public Transform GetTarget() => target;
    public EnemyAttack GetAttackModule() => enemyAttack;

    public bool IsPlayerInChaseRange()
    {
        if (target == null) 
            return false;

        return Vector3.Distance(transform.position, target.position) <= chaseRange;
    }

    public bool IsPlayerInAttackRange()
    {
        if (target == null) 
            return false;

        Vector3 a = transform.position;
        Vector3 b = target.position;

        a.y = 0;
        b.y = 0;

        return Vector3.Distance(a, b) <= attackRange;
    }

    public void AttackPlayer()
    {
        enemyAttack?.TryAttack();
    }

    #endregion

    #region Magnetismo

    public void SetMagnetize(bool active, bool attracted = false)
    {
        if (active)
        {
            magnetizeCount++;

            // Solo permitir cambiar el flag SI NO está repelido
            if (!isRepelled)
            {
                isAttracted = attracted;
                if (attracted) isRepelled = false;
            }

            magnetizedStartTime = Time.time;
        }
        else
        {
            magnetizeCount = Mathf.Max(0, magnetizeCount - 1);
            if (magnetizeCount == 0)
            {
                isAttracted = false;
                isRepelled = false;
            }
        }
    }

    public void SetRepelled()
    {
        isRepelled = true;
        isAttracted = false;
        wasRepelledOnce = true;
        SetMagnetize(true, attracted: false);
    }

    public bool CanResumeFSM()
    {
        if (isRepelled)
        {
            if (!hasCollidedWhileMagnetized)
                return false;

            float currentY = transform.position.y;
            float targetY = initialHeight;
            float deltaY = Mathf.Abs(currentY - targetY);
        }

        if (isAttracted)
        {
            return Time.time - magnetizedStartTime >= 2.5f;
        }

        return !IsMagnetized();
    }

    #endregion

    #region Patrullaje

    public Transform GetCurrentPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return null;

        return patrolPoints[currentPatrolIndex];
    }

    public void GoToNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    public void SetClosestPatrolPointAsCurrent()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        float closestDist = Mathf.Infinity;
        int closestIndex = 0;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestIndex = i;
            }
        }

        currentPatrolIndex = closestIndex;
    }

    #endregion

    public void StopAllMovement()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
