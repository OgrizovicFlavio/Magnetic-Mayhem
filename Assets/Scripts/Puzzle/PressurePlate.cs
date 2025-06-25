using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Transform plateVisual;
    [SerializeField] private float pressedDepth = 0.5f;
    [SerializeField] private float pressedSpeed = 10f;
    [SerializeField] private GameObject blockedObject;
    [SerializeField] private LayerMask validLayer;

    private Vector3 initialPosition;
    private int validObjectsOnPlate = 0;

    private void Start()
    {
        if (plateVisual == null)
        {
            plateVisual = transform;
        }

        initialPosition = plateVisual.localPosition;
        UpdateBlockedObject();
    }

    private void Update()
    {
        Vector3 targetPosition = initialPosition;

        if (validObjectsOnPlate > 0)
            targetPosition.y -= pressedDepth;

        plateVisual.localPosition = Vector3.Lerp(plateVisual.localPosition, targetPosition, Time.deltaTime * pressedSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Utilities.CheckLayerInMask(validLayer, other.gameObject.layer))
        {
            validObjectsOnPlate++;
            UpdateBlockedObject();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Utilities.CheckLayerInMask(validLayer, other.gameObject.layer))
        {
            validObjectsOnPlate = Mathf.Max(0, validObjectsOnPlate - 1);
            UpdateBlockedObject();
        }
    }

    private void UpdateBlockedObject()
    {
        bool isPressed = validObjectsOnPlate > 0;
        if (blockedObject != null)
            blockedObject.SetActive(!isPressed);
    }
}
