using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class MagnetDetector : MonoBehaviour
{
    public event Action<Magnet> onMagnetEnter;
    public event Action<Magnet> onMagnetExit;

    private void OnTriggerEnter(Collider other)
    {
        Magnet magnet = other.GetComponent<Magnet>();
        if (magnet != null)
        {
            onMagnetEnter?.Invoke(magnet);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Magnet magnet = other.GetComponent<Magnet>();
        if (magnet != null)
        {
            onMagnetExit?.Invoke(magnet);
        }
    }
}
