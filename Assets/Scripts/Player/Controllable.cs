using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Controllable : MonoBehaviour, IControllable
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject visual;
    [SerializeField] private GameObject controllerReceiver;

    private Outline outline;

    public GameObject Visual => visual;

    public GameObject GetControllerReceiver()
    {
        return controllerReceiver;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        gameObject.layer = LayerMask.NameToLayer("Magnetic");

        outline = GetComponentInChildren<Outline>();
        if (outline != null)
            outline.enabled = false;

        if (visual == null)
        {
            visual = new GameObject("Visual");
            visual.transform.parent = transform;
            visual.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        if (controllerReceiver == null)
        {
            controllerReceiver = new GameObject("Controller Receiver");
            controllerReceiver.transform.parent = transform;
            controllerReceiver.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }

    public void ControlEntity(PlayerController controller)
    {
        controller.GetRigidbody().velocity = Vector3.zero;

        controller.transform.SetParent(transform);
        controller.transform.localPosition = Vector3.zero;

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.rotation = Quaternion.identity;

        controller.SetRigidbody(rb);
    }

    public void SetVisualToController(PlayerController controller)
    {
        Transform visualDelPlayer = controller.transform.GetChild(0);
        if (visualDelPlayer != null)
            visualDelPlayer.gameObject.SetActive(true);
    }

    public void SetOutline(bool active)
    {
        if (outline != null)
            outline.enabled = active;
    }
}
