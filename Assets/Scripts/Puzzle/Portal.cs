using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private string promptMessage = "Presioná E para entrar";
    [SerializeField] private string sceneToLoad;
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform returnPoint;
    [SerializeField] private bool isExitPortal = false;

    private bool canInteract = false;

    private void Start()
    {
        if (playerRoot == null)
        {
            playerRoot = GameManager.Instance.GetPlayerRoot();
            if (playerRoot == null)
                Debug.LogWarning("[Portal] No se encontró el PlayerRoot.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Utilities.CheckLayerInMask(playerLayer, other.gameObject.layer))
        {
            canInteract = true;
            Debug.Log("[Portal] Jugador en rango. Mostrar mensaje: " + promptMessage);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Utilities.CheckLayerInMask(playerLayer, other.gameObject.layer))
        {
            canInteract = false;
        }
    }

    private void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            if (isExitPortal)
                GameManager.Instance.UnloadScene();
            else
                GameManager.Instance.LoadScene(sceneToLoad);
        }
    }
}


