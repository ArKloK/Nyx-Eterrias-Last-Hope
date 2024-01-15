using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour
{
    public Transform respawnPoint;
    public static RespawnController instance;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.position = respawnPoint.position;
            other.GetComponent<PlayerController>().TakeDamage(1);
        }
    }
}
