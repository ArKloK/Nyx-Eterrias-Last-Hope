using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    public PlayerController playerController;
    void OnEnable()
    {
        HealthSphere.OnHealthSphereCollected += HealthSphereCollected;
    }

    void OnDisable()
    {
        HealthSphere.OnHealthSphereCollected -= HealthSphereCollected;
    }

    private void HealthSphereCollected()
    {
        playerController.Heal(1);
        Debug.Log("HealthSphere collected from CollectionManager.cs");
    }
}
