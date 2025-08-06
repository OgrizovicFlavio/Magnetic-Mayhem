using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody rb;
    private Vector3 currentVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Mueve al enemigo hacia una posición deseada, manteniendo una altura fija.
    /// </summary>
    public void MoveTowards(Vector3 targetPosition, float desiredHeight = -17f)
    {
        Vector3 correctedTarget = targetPosition;
        correctedTarget.y = desiredHeight;

        Vector3 direction = (correctedTarget - transform.position);
        direction.y = 0f;
        direction = direction.normalized;

        if (direction == Vector3.zero) return;

        Vector3 desiredVelocity = direction * moveSpeed;
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        Vector3 smoothVelocity = Vector3.SmoothDamp(flatVelocity, desiredVelocity, ref currentVelocity, 0.1f);

        rb.velocity = new Vector3(smoothVelocity.x, rb.velocity.y, smoothVelocity.z);
    }

    /// <summary>
    /// Rota suavemente al enemigo hacia la posición deseada.
    /// </summary>
    public void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0f;

        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    /// <summary>
    /// Detiene todo el movimiento del enemigo.
    /// </summary>
    public void StopMovement()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        currentVelocity = Vector3.zero;
    }
}

