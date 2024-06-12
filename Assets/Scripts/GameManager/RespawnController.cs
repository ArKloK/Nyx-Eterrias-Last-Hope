using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour
{
    public Transform respawnPoint;
    public static RespawnController Instance;
    private int playerHealth;
    private int playerLevel;
    private int playerExperience;

    public int PlayerHealth { get => playerHealth; set => playerHealth = value; }
    public int PlayerLevel { get => playerLevel; set => playerLevel = value; }
    public int PlayerExperience { get => playerExperience; set => playerExperience = value; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.position = respawnPoint.position;
            SetPlayerStats();
            other.GetComponent<PlayerController>().TakeDamage();
        }
    }

    public void SetPlayerStats()
    {
        PlayerStats.CurrentHealthPoints = playerHealth;
        PlayerStats.CurrentLevel = playerLevel;
        PlayerStats.CurrentExperiencePoints = playerExperience;
    }
}
