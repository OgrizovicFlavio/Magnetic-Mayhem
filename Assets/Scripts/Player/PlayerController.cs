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

    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Camera Culling")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask playerBody;

    private PlayerFSM playerFSM;
    private Rigidbody rb;
    private Rigidbody originalRb;
    private Transform originalBody;
    private CameraTransition cameraTransition;
    private RigidbodyConstraints originalConstraints;
    private LayerMask defaultCullingMask;

    private Vector3 originalCameraLocalPos;
    private Quaternion originalCameraLocalRot;
    private Vector3 originalControllerLocalPos;

    private bool isControllingProp = false;
    private bool hasSavedInitialPos = false;

    private void Start()
    {
        GameManager.Instance.RegisterPlayer(transform.parent.gameObject);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cameraTransition = new CameraTransition(cameraHolder, 0.4f);
        defaultCullingMask = mainCamera.cullingMask;

        if (transform.parent != null)
            rb = transform.parent.GetComponent<Rigidbody>();

        InputHandler input = new InputHandler();

        movement.Initialize(rb, cameraHolder);
        look.Initialize(transform, cameraHolder, visualTransform);
        shooter.Initialize(cameraHolder, this);
        playerFSM = new PlayerFSM(this, input, shooter.Shoot, shooter.ToggleCharge);
        RegisterOriginalBody(transform.parent, rb, playerFSM);
    }

    private void LateUpdate()
    {
        if (!hasSavedInitialPos)
        {
            originalCameraLocalPos = cameraHolder.localPosition;
            originalCameraLocalRot = cameraHolder.localRotation;
            originalControllerLocalPos = transform.localPosition;
            hasSavedInitialPos = true;
        }
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
                movement.SetMoveInput(input.GetMoveInput());
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

                Transform visual = hit.transform.Find("Visual");
                if (visual != null)
                    look.SetVisualTarget(visual);

                Transform receiver = hit.transform.Find("Controller Receiver");
                if (receiver != null)
                {
                    transform.parent = receiver;
                    transform.localPosition = Vector3.zero;
                    cameraHolder.localPosition = Vector3.zero;
                    cameraHolder.localRotation = Quaternion.identity;
                }

                shooter.enabled = false;
                isControllingProp = true;
                mainCamera.cullingMask |= playerBody;
            }
        }
    }

    public void SetRigidbody(Rigidbody newRb) => rb = newRb;
    public Animator GetAnimator() => animator;
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
        transform.localPosition = originalControllerLocalPos;

        if (transform.parent.TryGetComponent<Rigidbody>(out var propRb))
            propRb.constraints = RigidbodyConstraints.None;

        rb = originalRb;
        rb.constraints = originalConstraints;
        movement.SetRigidbody(originalRb);

        look.SetVisualTarget(visualTransform);

        shooter.enabled = true;
        isControllingProp = false;

        cameraHolder.localPosition = originalCameraLocalPos;
        cameraHolder.localRotation = originalCameraLocalRot;

        cameraTransition?.StartTransition(originalBody);
        mainCamera.cullingMask = defaultCullingMask;
    }

    public bool IsGrounded()
    {
        return movement.IsGrounded();
    }
}
