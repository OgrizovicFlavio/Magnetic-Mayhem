using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private PlayerShoot shooter;

    [Header("Movement & Look")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerLook look;

    private PlayerFSM playerFSM;
    private Rigidbody rb;
    private Rigidbody originalRb;
    private Transform originalBody;
    private RigidbodyConstraints originalConstraints;
    private CameraTransition cameraTransition;
    private Vector3 originalCameraPos;
    private Quaternion originalCameraRot;

    private bool isControllingProp = false;

    private void Start()
    {
        GameManager.Instance.RegisterPlayer(transform.parent.gameObject);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cameraTransition = new CameraTransition(cameraHolder, 0.4f);
        originalCameraPos = cameraHolder.localPosition;
        originalCameraRot = cameraHolder.localRotation;

        if (transform.parent != null)
            rb = transform.parent.GetComponent<Rigidbody>();

        InputHandler input = new InputHandler();

        movement.Initialize(rb, cameraHolder);
        look.Initialize(transform, cameraHolder);
        shooter.Initialize(cameraHolder, this);
        playerFSM = new PlayerFSM(this, input, shooter.Shoot, shooter.ToggleCharge);
        RegisterOriginalBody(transform.parent, rb, playerFSM);
    }

    private void Update()
    {
        if (cameraTransition.IsActive)
        {
            cameraTransition.Update();
            return;
        }

        if (movement.IsGrounded())
            movement.ApplyGravity();

        InputHandler input = playerFSM?.GetInput();
        if (input != null)
        {
            look.Rotate(input.GetLookInput());

            if (isControllingProp)
            {
                SetMoveInput(input.GetMoveInput());

                if (input.IsInteracting())
                    ReturnToPlayer();
            }
            else
            {
                playerFSM.OnUpdate();
            }
        }
    }

    private void FixedUpdate()
    {
        movement.Move();
    }

    public void SetMoveInput(Vector2 moveInput)
    {
        movement.SetMoveInput(moveInput);
    }

    public void Jump()
    {
        if (movement.IsGrounded())
        {
            movement.Jump();
        }
    }

    public void TryPossess()
    {
        Ray ray = new Ray(cameraHolder.position, cameraHolder.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 20f))
        {
            if (hit.collider.TryGetComponent<IControllable>(out var controllable))
            {
                cameraTransition.StartTransition(hit.transform);
                controllable.ControlEntity(this);

                if (rb != null)
                    movement.SetRigidbody(rb);

                shooter.enabled = false;
                isControllingProp = true;
            }
        }
    }

    public void SetRigidbody(Rigidbody newRb) => rb = newRb;
    public Rigidbody GetRigidbody() => rb;
    public Transform GetCameraHolder() => cameraHolder;

    public void RegisterOriginalBody(Transform body, Rigidbody rb, PlayerFSM fsm)
    {
        originalConstraints = rb.constraints;
        originalBody = body;
        originalRb = rb;
    }

    public void ReturnToPlayer()
    {
        transform.parent = originalBody;
        transform.localPosition = Vector3.zero;

        if (transform.parent.TryGetComponent<Rigidbody>(out var propRb))
            propRb.constraints = RigidbodyConstraints.None;

        rb = originalRb;
        rb.constraints = originalConstraints;
        movement.SetRigidbody(originalRb);

        shooter.enabled = true;
        isControllingProp = false;

        cameraTransition?.StartTransition(originalBody);
    }
}
