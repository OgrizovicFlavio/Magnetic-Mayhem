using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Sensitivity")]
    [SerializeField] private float mouseSensitivity = 200f;

    [Header("Vertical Look Limit")]
    [SerializeField] private float minVerticalAngle = -40f;
    [SerializeField] private float maxVerticalAngle = 80f;

    private Transform visualTransform;
    private Transform bodyTransform;
    private Transform cameraHolderTransform;

    private float visualTurnSpeed = 10f;
    private float verticalLookRotation = 0f;
    private bool isFrozen = false;

    public void Initialize(Transform bodyTransform, Transform cameraHolderTransform, Transform visualTransform)
    {
        this.bodyTransform = bodyTransform;
        this.cameraHolderTransform = cameraHolderTransform;
        this.visualTransform = visualTransform;
    }

    public void Rotate(Vector2 lookInput)
    {
        if (isFrozen || bodyTransform == null || cameraHolderTransform == null || visualTransform == null)
            return;

        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        bodyTransform.Rotate(Vector3.up * mouseX);

        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, minVerticalAngle, maxVerticalAngle);
        cameraHolderTransform.localEulerAngles = new Vector3(verticalLookRotation, 0f, 0f);

        if (visualTransform != null)
        {
            Vector3 forward = bodyTransform.forward;
            Quaternion targetRotation = Quaternion.LookRotation(forward);
            visualTransform.rotation = Quaternion.Slerp(visualTransform.rotation, targetRotation, Time.deltaTime * visualTurnSpeed);
        }
    }

    public void SetVisualTarget(Transform newTarget)
    {
        visualTransform = newTarget;
    }

    public void SetFrozen(bool frozen)
    {
        isFrozen = frozen;
    }
}
