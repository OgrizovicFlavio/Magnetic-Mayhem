using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private string sceneToLoad;
    [SerializeField] private bool isExitPortal = false;

    private Transform playerRoot;
    private bool canInteract = false;

    private void OnEnable()
    {
        GameManager.OnPlayerRegistered += SetPlayerRoot;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerRegistered -= SetPlayerRoot;
    }

    private void SetPlayerRoot(GameObject player)
    {
        playerRoot = player.transform;
    }


    private void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            if (isExitPortal)
                GameManager.Instance.GoToMain();
            else
                GameManager.Instance.GoToArea(sceneToLoad);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Utilities.CheckLayerInMask(playerLayer, other.gameObject.layer))
        {
            canInteract = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Utilities.CheckLayerInMask(playerLayer, other.gameObject.layer))
        {
            canInteract = false;
        }
    }
}


