using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthSphere : MonoBehaviour, ICollectible
{
    public static event Action OnHealthSphereCollected;

    public void Collect()
    {
        OnHealthSphereCollected?.Invoke();
        Destroy(gameObject);
    }
}
