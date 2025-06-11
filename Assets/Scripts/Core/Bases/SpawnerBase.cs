using UnityEngine;

public abstract class SpawnerBase<T> : MonoBehaviour where T : MonoBehaviour, IPooleable
{
    public abstract void Spawn(Vector3 position);
}
