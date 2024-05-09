using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    //[SerializeField] bool TBDemo;
    //[SerializeField] List<Collectible> collectibles;
    // void Start()
    // {
    //     if (TBDemo)
    //     {
    //         foreach (ICollectible collectible in collectibles)
    //         {
    //             collectible.Collect();
    //         }
    //     }
    // }
    void OnTriggerEnter2D(Collider2D other)
    {
        ICollectible collectible = other.GetComponent<ICollectible>();
        if (collectible != null)
        {
            collectible.Collect();
        }
    }
}
