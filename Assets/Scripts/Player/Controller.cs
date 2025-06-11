using UnityEngine;

public class Controller : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraHolder;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float mouseSensitivity = 200f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Slope Detection")]
    [SerializeField] private float maxSlopeAngle = 40f;
    [SerializeField] private float slopeCheckDistance = 5f;

    private Rigidbody rb;
    private Vector2 currentMoveInput;
    private Vector3 currentVelocity = Vector3.zero;
    private IPlayerInput input;
    private MagneticChargeType currentCharge = MagneticChargeType.Positive;
    private float verticalLookRotation = 0f;
    private bool isGrounded = false;

    public void SetInput(IPlayerInput inputSource)
    {
        input = inputSource;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        isGrounded = CheckIfGrounded();

        //Aumenta la gravedad si está cayendo
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        //Salto "corto"
        else if (rb.velocity.y > 0 && !input.IsJumping())
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        Move(currentMoveInput);
    }

    public void SetMoveInput(Vector2 moveInput)
    {
        currentMoveInput = moveInput;
    }

    public void RotateCamera(Vector2 lookInput)
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -40f, 90f);

        cameraHolder.localEulerAngles = new Vector3(verticalLookRotation, 0f, 0f);
    }

    private void Move(Vector2 moveInput)
    {
        Vector3 moveDir = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;

        if (!CanMove(moveDir))
            return;

        Vector3 desiredVelocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed);

        rb.velocity = Vector3.SmoothDamp(rb.velocity, desiredVelocity, ref currentVelocity, 0.1f);
    }

    private bool CanMove(Vector3 moveDir)
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null) 
            return true; //si no hay terreno, que se pueda mover

        Vector3 relativePos = GetMapPos();
        Vector3 normal = terrain.terrainData.GetInterpolatedNormal(relativePos.x, relativePos.z);
        float angle = Vector3.Angle(normal, Vector3.up);

        float currentHeight = terrain.SampleHeight(rb.position);
        float nextHeight = terrain.SampleHeight(rb.position + moveDir * slopeCheckDistance);

        //Si la pendiente es muy empinada y estamos subiendo, no se puede mover
        if (angle > maxSlopeAngle && nextHeight > currentHeight)
            return false;

        return true;
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

    public void Jump()
    {
        if (!isGrounded) return;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    public void ToggleCharge()
    {
        if (currentCharge == MagneticChargeType.Positive)
            currentCharge = MagneticChargeType.Negative;
        else
            currentCharge = MagneticChargeType.Positive;
        Debug.Log("Nueva carga: " + currentCharge);
    }

    public void TryPossess()
    {
        if (input == null || !input.IsInteracting()) 
            return;

        Ray ray = new Ray(cameraHolder.position, cameraHolder.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f))
        {
            if (hit.collider.TryGetComponent<IControllable>(out var controllable))
            {
                controllable.ControlEntity(this);
            }
        }
    }

    private bool CheckIfGrounded()
    {
        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, groundDistance, groundLayer);
        return colliders.Length > 0;
    }

    public MagneticChargeType GetCurrentCharge()
    {
        return currentCharge;
    }
}
