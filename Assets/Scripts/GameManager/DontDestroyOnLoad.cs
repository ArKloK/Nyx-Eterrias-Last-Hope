using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoadScript : MonoBehaviour
{
    private static Dictionary<string, DontDestroyOnLoadScript> instances = new Dictionary<string, DontDestroyOnLoadScript>();

    private void Awake()
    {
        if(SceneManager.GetActiveScene().name == "Main Menu")
        {
            Destroy(gameObject);
            return;
        }
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

    public static void DestroyAll()
    {
        foreach (var instance in instances.Values)
        {
            Destroy(instance.gameObject);
        }
        instances.Clear();
    }
}
