using UnityEngine;

public class Activator : MonoBehaviour, IActivable
{
    [SerializeField] private GameObject[] objectsToActivate;
    [SerializeField] private GameObject[] objectsToDeactivate;

    public void Activate()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null && !obj.activeSelf)
                obj.SetActive(true);
        }

        foreach (GameObject obj in objectsToDeactivate)
        {
            if (obj != null && obj.activeSelf)
                obj.SetActive(false);
        }
    }

    public void Deactivate()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null && obj.activeSelf)
                obj.SetActive(false);
        }

        foreach (GameObject obj in objectsToDeactivate)
        {
            if (obj != null && !obj.activeSelf)
                obj.SetActive(true);
        }
    }
}
