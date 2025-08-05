using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private PlayerShoot shooter;
    [SerializeField] private Transform visualTransform;

    [Header("Movement & Look")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerLook look;
    [SerializeField] private CrosshairController crosshairController;
    [SerializeField] private Camera mainCamera;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    private PlayerFSM playerFSM;
    private Rigidbody rb;
    private Rigidbody originalRb;
    private Transform originalBody;
    private CameraTransition cameraTransition;
    private RigidbodyConstraints originalConstraints;
    private float jumpCooldownTimer = 0f;
    private float jumpCooldownDuration = 0.1f;

    private bool isControllingProp = false;
    private Controllable lastHighlighted = null;

    private void Start()
    {
        GameManager.Instance.RegisterPlayer(transform.parent.gameObject);

        if (mainCamera != null)
        {
            mainCamera.transform.localPosition = new Vector3(0f, 1f, -7f);
            mainCamera.transform.localRotation = Quaternion.identity;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cameraTransition = new CameraTransition(cameraHolder, 0.4f);

        if (transform.parent != null)
            rb = transform.parent.GetComponent<Rigidbody>();

        InputHandler input = new InputHandler();

        movement.Initialize(rb, cameraHolder);
        look.Initialize(transform, cameraHolder, visualTransform);
        shooter.Initialize(cameraHolder, this);
        playerFSM = new PlayerFSM(this, input, shooter.Shoot, shooter.ToggleCharge);
        RegisterOriginalBody(transform.parent, rb);
    }

    private void Update()
    {
        bool inTransition = cameraTransition.IsActive;

        look.SetFrozen(inTransition);
        movement.SetFrozen(inTransition);

        if (inTransition)
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

            Vector2 moveInput = input.GetMoveInput();
            SetMoveInput(moveInput);

            if (isControllingProp)
            {
                if (input.IsInteracting())
                    ReturnToPlayer();
            }
            else
            {
                playerFSM.OnUpdate();
                CheckInteractable();
            }

            if (jumpCooldownTimer > 0f)
                jumpCooldownTimer -= Time.deltaTime;

            bool grounded = IsGrounded();
            bool inJumpCooldown = jumpCooldownTimer > 0f;
            bool isMoving = IsMoving();
            bool canShoot = shooter.CanShootNow() && grounded && !inJumpCooldown && !isMoving;

            crosshairController.SetCanShoot(canShoot);
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
            jumpCooldownTimer = jumpCooldownDuration;
        }
    }

    public void TryPossess()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            if (hit.collider.TryGetComponent<IControllable>(out var controllable))
            {
                cameraTransition.StartTransition(hit.transform);
                controllable.ControlEntity(this);

                if (hit.collider.TryGetComponent<Controllable>(out var ctrl))
                {
                    var propRb = ctrl.GetComponent<Rigidbody>();
                    SetRigidbody(propRb);
                    movement.SetRigidbody(propRb);

                    if (ctrl.Visual != null)
                        look.SetVisualTarget(ctrl.Visual.transform);

                    if (ctrl.GetControllerReceiver() != null)
                    {
                        transform.SetParent(ctrl.GetControllerReceiver().transform);
                        transform.localPosition = Vector3.zero;
                        cameraHolder.localPosition = Vector3.zero;
                        cameraHolder.localRotation = Quaternion.identity;
                    }
                }

                animator.SetFloat("MoveX", 0f);
                animator.SetFloat("MoveY", 0f);
                animator.SetInteger("State", (int)PlayerState.Idle);
                animator.Rebind();
                animator.enabled = false;

                shooter.enabled = false;
                crosshairController.SetActive(false);
                isControllingProp = true;
            }
        }
    }

    public void RegisterOriginalBody(Transform body, Rigidbody rb)
    {
        originalConstraints = rb.constraints;
        originalBody = body;
        originalRb = rb;
    }

    public void ReturnToPlayer()
    {
        transform.SetParent(originalBody);
        transform.localPosition = new Vector3(0f, 3f, 0f);

        if (transform.parent.TryGetComponent<Rigidbody>(out var propRb))
            propRb.constraints = RigidbodyConstraints.None;

        rb = originalRb;
        rb.constraints = originalConstraints;
        movement.SetRigidbody(originalRb);

        look.SetVisualTarget(visualTransform);

        animator.SetFloat("MoveX", 0f);
        animator.SetFloat("MoveY", 0f);
        animator.SetInteger("State", (int)PlayerState.Idle);
        animator.enabled = true;

        shooter.enabled = true;
        crosshairController.SetActive(true);
        isControllingProp = false;

        cameraHolder.SetParent(transform);
        cameraHolder.localPosition = Vector3.zero;
        cameraHolder.localRotation = Quaternion.identity;

        InputHandler input = playerFSM?.GetInput();
        if (input != null)
        {
            Vector2 move = input.GetMoveInput();
            animator.SetFloat("MoveX", move.x);
            animator.SetFloat("MoveY", move.y);
            animator.SetInteger("State", move.magnitude > 0.1f ? (int)PlayerState.Run : (int)PlayerState.Idle);
            animator.Update(0f);
        }
    }
    private void CheckInteractable()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, 25f))
        {
            var c = hit.collider.GetComponentInParent<Controllable>();
            if (c != null)
            {
                UIManager.Instance?.ShowInteractionHint("POSSESS: E");

                if (c != lastHighlighted)
                {
                    lastHighlighted?.SetOutline(false);
                    c.SetOutline(true);
                    lastHighlighted = c;
                }

                return;
            }

            if (hit.collider.TryGetComponent<Portal>(out var portal))
            {
                string msg = portal.IsExit() ? "EXIT: E" : "ENTER: E";
                UIManager.Instance?.ShowInteractionHint(msg);

                lastHighlighted?.SetOutline(false);
                lastHighlighted = null;
                return;
            }
        }

        lastHighlighted?.SetOutline(false);
        lastHighlighted = null;
        UIManager.Instance?.HideInteractionHint();
    }

    public bool IsGrounded() => movement.IsGrounded();

    public bool IsMoving()
    {
        InputHandler input = playerFSM?.GetInput();
        if (input == null) return false;

        Vector2 move = input.GetMoveInput();
        return move.magnitude > 0.1f;
    }

    public void SetRigidbody(Rigidbody newRb) => rb = newRb;
    public Animator GetAnimator() => animator;
    public Rigidbody GetRigidbody() => rb;
    public Transform GetCameraHolder() => cameraHolder;
    public CrosshairController GetCrosshairController() => crosshairController;
}
