using UnityEngine;

public class Controllable : MonoBehaviour, IControllable
{
    [SerializeField] private GameObject visual;

    public void ControlEntity(PlayerController controller)
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        //Resetear velocidad del controlador antes de tomar control
        controller.GetRigidbody().velocity = Vector3.zero;

        //Reparentar al nuevo cuerpo
        controller.transform.SetParent(transform);
        controller.transform.localPosition = Vector3.zero;

        //Conservar rotación y congelar giros físicos
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.rotation = Quaternion.identity;

        //Transferir Rigidbody al controller
        controller.SetRigidbody(rb);
    }

    public void SetVisualToController(PlayerController controller)
    {
        Transform visualDelPlayer = controller.transform.GetChild(0);
        if (visualDelPlayer != null)
            visualDelPlayer.gameObject.SetActive(true);
    }
}
