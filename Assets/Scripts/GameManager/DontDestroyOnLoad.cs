using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    private static Dictionary<string, DontDestroyOnLoad> instances = new Dictionary<string, DontDestroyOnLoad>();

    private void Awake()
    {
        if (!instances.ContainsKey(gameObject.name))
        {
            instances[gameObject.name] = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instances[gameObject.name] != this)
        {
            Destroy(gameObject);
        }
    }
}
