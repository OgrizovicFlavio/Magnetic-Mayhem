using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneReferences : MonoBehaviour
{
    public static event Action<SceneReferences> OnLoadedScene;

    [field: SerializeField] public Transform returnPoint { get; private set; }
    [field: SerializeField] public List<GameObject> gameObjects { get; private set; } = new List<GameObject>();

    private void Start()
    {
        OnLoadedScene?.Invoke(this);
    }

    public void SetActiveGameObjects(bool state)
    {
        foreach (GameObject worldObject in gameObjects)
        {
            worldObject.SetActive(state);
        }
    }
}
