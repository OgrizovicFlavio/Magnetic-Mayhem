using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviourSingleton<PoolManager>
{
    private Dictionary<Type, IPooleable> prefabLookup = new();
    private Dictionary<Type, Queue<IPooleable>> pool = new();

    /*protected override void OnAwaken()
    {
        
    }*/

    public void InitializePool<T>(T prefab, int size) where T : MonoBehaviour, IPooleable
    {
        Type type = typeof(T);

        if (!prefabLookup.ContainsKey(type))
            prefabLookup[type] = prefab;

        if (!pool.ContainsKey(type))
            pool[type] = new Queue<IPooleable>();

        for (int i = 0; i < size; i++)
        {
            T instance = Instantiate(prefab, transform);
            instance.ResetToDefault();
            instance.gameObject.SetActive(false);
            pool[type].Enqueue(instance);
        }
    }

    public T Get<T>(Vector3 position, Quaternion rotation) where T : MonoBehaviour, IPooleable
    {
        Type type = typeof(T);
        IPooleable obj;

        if (pool.ContainsKey(type) && pool[type].Count > 0)
        {
            obj = pool[type].Dequeue();
            obj.ResetToDefault();
        }
        else if (prefabLookup.ContainsKey(type))
        {
            obj = Instantiate((prefabLookup[type] as MonoBehaviour).gameObject).GetComponent<IPooleable>();
        }
        else
        {
            return null;
        }

        var go = (obj as MonoBehaviour).gameObject;
        go.transform.SetPositionAndRotation(position, rotation);
        go.SetActive(true);

        obj.GetObjectFromPool();
        return obj as T;
    }

    public void ReturnToPool(IPooleable obj)
    {
        obj.ReturnObjectToPool();
        obj.Disable();

        GameObject go = (obj as MonoBehaviour).gameObject;
        go.transform.SetParent(transform);
        go.SetActive(false);

        Type type = obj.GetType();
        if (!pool.ContainsKey(type))
            pool[type] = new Queue<IPooleable>();

        pool[type].Enqueue(obj);
    }

    public void ReturnAll()
    {
        foreach (var queue in pool.Values)
        {
            foreach (var obj in queue)
            {
                obj.Disable();
                (obj as MonoBehaviour).gameObject.SetActive(false);
            }
        }
    }
}
