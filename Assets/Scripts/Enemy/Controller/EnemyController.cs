using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyData Data;
    private EnemyMovementController enemyMovementController;
    private int currentHealthPoints;

    public int CurrentHealthPoints { get => currentHealthPoints; set => currentHealthPoints = value; }

    void Awake()
    {
        currentHealthPoints = Data.MaxHealthPoints;
    }
    void Start()
    {
        enemyMovementController = GetComponent<EnemyMovementController>();
    }
    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        currentHealthPoints -= damage;
        if (currentHealthPoints <= 0)
        {
            Die();
        }else{
            Vector2 knockBackDirection = ((Vector2)transform.position - attackerPosition).normalized;
            StartCoroutine(enemyMovementController.LoseControl());
            enemyMovementController.KnockBack(knockBackDirection);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(other.GetContact(0).normal);

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
