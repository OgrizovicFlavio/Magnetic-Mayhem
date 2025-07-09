using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Slope Detection")]
    [SerializeField] private float maxSlopeAngle = 40f;
    [SerializeField] private float slopeCheckDistance = 5f;

    private Rigidbody rb;
    private Transform cameraTransform;

    private Vector2 moveInput = Vector2.zero;
    private Vector3 currentVelocity = Vector3.zero;
    private bool isGrounded = false;

    public void Initialize(Rigidbody rb, Transform cameraTransform)
    {
        this.rb = rb;
        this.cameraTransform = cameraTransform;
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void Move()
    {
        if (rb == null || cameraTransform == null) return;

        isGrounded = CheckIfGrounded();

        ApplyGravity();

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (right * moveInput.x + forward * moveInput.y).normalized;

        if (!CanMove(moveDir))
            return;

        Vector3 desiredVelocity = rb.velocity;

        if (isGrounded || !IsTouchingWall())
        {
            desiredVelocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, desiredVelocity, ref currentVelocity, 0.1f);
        }
    }

    public void Jump()
    {
        if (!isGrounded || rb == null) return;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    public void ApplyGravity()
    {
        if (rb == null) return;

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private bool CheckIfGrounded()
    {
        bool grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
        return grounded;
    }

    private bool IsTouchingWall()
    {
        float radius = 0.3f;
        float castDistance = 0.6f;
        Vector3 origin = rb.position + Vector3.up * 0.5f;

        return Physics.SphereCast(origin, radius, transform.forward, out _, castDistance, wallLayer)
            || Physics.SphereCast(origin, radius, -transform.forward, out _, castDistance, wallLayer)
            || Physics.SphereCast(origin, radius, transform.right, out _, castDistance, wallLayer)
            || Physics.SphereCast(origin, radius, -transform.right, out _, castDistance, wallLayer);
    }

    private bool CanMove(Vector3 moveDir)
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null) return true;

        Vector3 relativePos = GetMapPos();
        Vector3 normal = terrain.terrainData.GetInterpolatedNormal(relativePos.x, relativePos.z);
        float angle = Vector3.Angle(normal, Vector3.up);

        float currentHeight = terrain.SampleHeight(rb.position);
        float nextHeight = terrain.SampleHeight(rb.position + moveDir * slopeCheckDistance);

        return angle <= maxSlopeAngle || nextHeight <= currentHeight;
    }

    private Vector3 GetMapPos()
    {
        Vector3 pos = rb.position;
        Terrain terrain = Terrain.activeTerrain;

        return new Vector3(
            (pos.x - terrain.transform.position.x) / terrain.terrainData.size.x,
            0,
            (pos.z - terrain.transform.position.z) / terrain.terrainData.size.z
        );
    }

    public bool IsGrounded() => isGrounded;

    public void SetRigidbody(Rigidbody newRb)
    {
        rb = newRb;
    }
}
