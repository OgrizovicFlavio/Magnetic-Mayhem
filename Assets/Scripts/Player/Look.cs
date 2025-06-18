using UnityEngine;

public class Look : MonoBehaviour
{
    [Header("Sensitivity")]
    [SerializeField] private float mouseSensitivity = 200f;

    [Header("Vertical Look Limit")]
    [SerializeField] private float minVerticalAngle = -40f;
    [SerializeField] private float maxVerticalAngle = 80f;

    private Transform bodyTransform;
    private Transform cameraHolderTransform;

    private float verticalLookRotation = 0f;

    public void Initialize(Transform bodyTransform, Transform cameraHolderTransform)
    {
        this.bodyTransform = bodyTransform;
        this.cameraHolderTransform = cameraHolderTransform;
    }

    public void Rotate(Vector2 lookInput)
    {
        if (bodyTransform == null || cameraHolderTransform == null)
        {
            Debug.LogWarning("Look: Referencias no inicializadas.");
            return;
        }

        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        // Rotación horizontal (Y)
        bodyTransform.Rotate(Vector3.up * mouseX);

        // Rotación vertical (X)
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, minVerticalAngle, maxVerticalAngle);
        cameraHolderTransform.localEulerAngles = new Vector3(verticalLookRotation, 0f, 0f);
    }
}
