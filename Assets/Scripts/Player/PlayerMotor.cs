using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraHolder;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float mouseSensitivity = 200f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Slope Detection")]
    [SerializeField] private float maxSlopeAngle = 40f;
    [SerializeField] private float slopeCheckDistance = 5f;

    private Rigidbody rb;
    private IPlayerInput input;
    private float verticalLookRotation = 0f;
    private bool isGrounded = false;
    private MagneticChargeType currentCharge = MagneticChargeType.Positive;
    private Vector2 currentMoveInput;

    public void SetInput(IPlayerInput inputSource)
    {
        input = inputSource;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        isGrounded = CheckIfGrounded();
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
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.localEulerAngles = new Vector3(verticalLookRotation, 0f, 0f);
    }

    private void Move(Vector2 moveInput)
    {
        Vector3 moveDir = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;

        if (!CanMove(moveDir))
            return;

        Vector3 velocity = rb.velocity;
        velocity.x = moveDir.x * moveSpeed;
        velocity.z = moveDir.z * moveSpeed;
        rb.velocity = velocity;
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

        Debug.Log($"Angle: {angle}");

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

    public void Shoot() //CONSULTAR
    {
        Debug.Log("Disparo con carga: " + currentCharge);
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
