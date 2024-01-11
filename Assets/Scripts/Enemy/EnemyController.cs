using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyData Data;

    [HideInInspector]
    public int currentHealthPoints;

    void Awake()
    {
        currentHealthPoints = Data.MaxHealthPoints;
    }
    public void TakeDamage(int damage)
    {
        currentHealthPoints -= damage;
        if (currentHealthPoints <= 0)
        {
            Die();
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(Data.AttackPower, other.GetContact(0).normal);
                Debug.Log("Player took damage"); 
            }
        }
    }

    public void Die()
    {
        ExperienceManager.Instance.AddExperience(Data.ExperiencePoints);
        this.gameObject.SetActive(false);
    }
}
