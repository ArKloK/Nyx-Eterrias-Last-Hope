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

    public void Die()
    {
        ExperienceManager.Instance.AddExperience(Data.ExperiencePoints);
        this.gameObject.SetActive(false);
    }
}
