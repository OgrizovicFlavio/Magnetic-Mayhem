using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float repulsionFallDelay = 2f;

    private Rigidbody rb;
    private Vector3 currentVelocity;
    private float initialHeight;
    private Coroutine fallCoroutine;
    private bool isRepelled = false;
    private bool hasCollidedWhileMagnetized = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        initialHeight = transform.position.y;
    }

    #region Movement

    public void MoveTowards(Vector3 targetPosition)
    {
        if (isRepelled) return;

        Vector3 direction = (targetPosition - transform.position);
        direction.y = 0f;
        direction = direction.normalized;

        if (direction == Vector3.zero)
            return;

        Vector3 desiredVelocity = direction * moveSpeed;
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        Vector3 smoothVelocity = Vector3.SmoothDamp(flatVelocity, desiredVelocity, ref currentVelocity, 0.1f);

        rb.velocity = new Vector3(smoothVelocity.x, rb.velocity.y, smoothVelocity.z);
    }

    public void RotateTowards(Vector3 targetPosition)
    {
        if (isRepelled) return;

        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0f;

        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    public void StopMovement()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        currentVelocity = Vector3.zero;
    }

    #endregion

    #region Attack
    public void RetreatFrom(Vector3 fromPosition, float force, float duration)
    {
        StartCoroutine(RetreatRoutine(fromPosition, force, duration));
    }

    private IEnumerator RetreatRoutine(Vector3 fromPosition, float force, float duration)
    {
        float elapsed = 0f;
        Vector3 retreatDir = (transform.position - fromPosition).normalized;
        retreatDir.y = 0f;

        while (elapsed < duration)
        {
            rb.AddForce(retreatDir * force * Time.deltaTime, ForceMode.VelocityChange);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    #endregion

    #region Magnetism

    public void ApplyRepulsion(Vector3 direction, float impulseForce)
    {
        StopMovement();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        isRepelled = true;
        rb.AddForce(direction.normalized * impulseForce, ForceMode.Impulse);

        if (fallCoroutine != null)
            StopCoroutine(fallCoroutine);

        fallCoroutine = StartCoroutine(ForceFallAfterDelay(repulsionFallDelay));
    }

    private IEnumerator ForceFallAfterDelay(float delay)
    {
        float t = 0f;
        while (t < delay)
        {
            if (hasCollidedWhileMagnetized)
                yield break;

            t += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(Vector3.down * 40f, ForceMode.VelocityChange);
    }

    public void SetRepelled(bool value) => isRepelled = value;
    public void RegisterMagnetizedCollision() => hasCollidedWhileMagnetized = true;
    public void ResetMagnetizedCollision() => hasCollidedWhileMagnetized = false;

    public bool UpdateHeight(float threshold = 0.1f)
    {
        Vector3 pos = transform.position;
        float deltaY = initialHeight - pos.y;

        if (Mathf.Abs(deltaY) < threshold)
            return true;

        float correctedY = Mathf.Lerp(pos.y, initialHeight, Time.deltaTime * 5f);
        transform.position = new Vector3(pos.x, correctedY, pos.z);
        return false;
    }

    #endregion
}

