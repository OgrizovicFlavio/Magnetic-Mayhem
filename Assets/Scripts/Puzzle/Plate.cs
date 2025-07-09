using UnityEngine;

public class Plate : MonoBehaviour
{
    [SerializeField] private GameObject objectToActivate;
    [SerializeField] private LayerMask magneticLayer;
    [SerializeField] private Animator animator;

    private int objectsInPlate = 0;
    private bool isPressed = false;


    private void OnTriggerEnter(Collider other)
    {
        if (Utilities.CheckLayerInMask(magneticLayer, other.gameObject.layer))
        {
            objectsInPlate++;

            if (!isPressed)
            {
                isPressed = true;
                objectToActivate?.GetComponent<IActivable>()?.Activate();
                animator?.SetBool("IsPressed", true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Utilities.CheckLayerInMask(magneticLayer, other.gameObject.layer))
        {
            objectsInPlate--;

            if (objectsInPlate <= 0)
            {
                isPressed = false;
                objectToActivate?.GetComponent<IActivable>()?.Deactivate();
                animator?.SetBool("IsPressed", false);
            }
        }
    }
}
