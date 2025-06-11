using UnityEngine;

public class Controllable : MonoBehaviour, IControllable
{
    [SerializeField] private GameObject visual;

    public void ControlEntity(Controller controller)
    {
        Transform parent = controller.transform.parent;
        if (parent != null && parent.TryGetComponent(out Controllable previousControllable))
        {
            if (previousControllable.visual != null)
            {
                previousControllable.visual.transform.parent = parent;
                previousControllable.visual.transform.localPosition = Vector3.zero;
            }
        }

        // Reubica el controller en este objeto
        controller.transform.parent = transform;
        controller.transform.localPosition = Vector3.zero;

        // Inserta el nuevo visual como hijo del controller
        visual.transform.parent = controller.transform;
        visual.transform.localPosition = Vector3.zero;

        // Opcional: congela rotaciones si tiene Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }

    public void OutlineEntity()
    {
        //Para después
    }
}
