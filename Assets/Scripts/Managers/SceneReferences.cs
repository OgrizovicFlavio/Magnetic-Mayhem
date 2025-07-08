using System.Collections.Generic;
using UnityEngine;

public class SceneReferences : MonoBehaviour
{
    [field: SerializeField] public Transform SpawnPoint { get; private set; }
    [field: SerializeField] public List<GameObject> gameObjects { get; private set; } = new();

    private void Start()
    {
        GameManager.Instance.MovePlayer(this);
    }

    public void SetActiveGameObjects(bool state)
    {
        foreach (GameObject worldObject in gameObjects)
        {
            worldObject.SetActive(state);
        }
    }
}
