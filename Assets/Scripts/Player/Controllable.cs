using UnityEngine;

public class Controllable : MonoBehaviour, IControllable
{
    [SerializeField] private GameObject visual;

    public void ControlEntity(PlayerController controller)
    {
        Rigidbody rb = GetComponent<Rigidbody>();

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
}
