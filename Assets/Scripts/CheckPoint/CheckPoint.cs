using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            RespawnController.Instance.respawnPoint = this.transform;
            RespawnController.Instance.PlayerHealth = PlayerStats.CurrentHealthPoints;
            RespawnController.Instance.PlayerLevel = PlayerStats.CurrentLevel;
            RespawnController.Instance.PlayerExperience = PlayerStats.CurrentExperiencePoints;
            boxCollider2D.enabled = false;
        }
    }
}
