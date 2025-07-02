using UnityEngine;

public abstract class BaseSpawner<T> : MonoBehaviour where T : MonoBehaviour, IPooleable
{
    public abstract void Spawn(Vector3 position);
}
