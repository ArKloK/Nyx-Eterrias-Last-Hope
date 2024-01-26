using System;
using System.Collections;
using UnityEngine;

public class PlayerEventArgs : EventArgs
{
    public PlayerController PlayerController { get; set; }

    public PlayerEventArgs(PlayerController player)
    {
        PlayerController = player;
    }
}

public class PlayerController : MonoBehaviour
{
    [Header("Script References")]
    public PlayerControllerData Data;
    public HealthBar healthBar;
    private PlayerMovementController.PlayerMovementController playerMovementController;

    #region Health and Energy Variables
    private int maxHealthPoints;
    private int currentHealthPoints;
    public int CurrentHealthPoints { get => currentHealthPoints; set => currentHealthPoints = value; }
    private int currentSpiritualEnergyPoints;
    #endregion

    #region Level Variables
    private int currentExperiencePoints;
    private int maxExperiencePoints;
    private int currentLevel;
    #endregion

    #region Events
    public static event EventHandler<PlayerEventArgs> OnPlayerDeath;
    #endregion

    void Awake()
    {
        maxHealthPoints = Data.MaxHealthPoints;
        currentHealthPoints = maxHealthPoints;
        currentSpiritualEnergyPoints = Data.MaxSpiritualEnergyPoints;
    }
    void Start()
    {
        playerMovementController = GetComponent<PlayerMovementController.PlayerMovementController>();

        currentExperiencePoints = 0;
        maxExperiencePoints = Data.MaxExperiencePoints;
        currentLevel = 1;

        healthBar.SetMaxHealth(maxHealthPoints);
    }

    void OnEnable()
    {
        ExperienceManager.OnExperienceChanged += HandleExperienceChanged;
    }

    void OnDisable()
    {
        ExperienceManager.OnExperienceChanged -= HandleExperienceChanged;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(1);
            Debug.Log("Player took damage, current health: " + currentHealthPoints);
        }

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.TakeDamage(Data.AttackPower);
                Debug.Log("Enemy took damage, current health: " + enemyController.currentHealthPoints);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealthPoints -= damage;
        healthBar.SetHealth(currentHealthPoints);
        if (currentHealthPoints <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        currentHealthPoints -= damage;
        healthBar.SetHealth(currentHealthPoints);
        if (currentHealthPoints <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(LoseControl());
            StartCoroutine(DisableCollider());
            playerMovementController.KnockBack(knockbackDirection);
        }

    }

    private IEnumerator LoseControl()
    {
        playerMovementController.canMove = false;
        yield return new WaitForSeconds(Data.LoseControlTime);
        playerMovementController.canMove = true;
    }

    private IEnumerator DisableCollider()
    {
        Physics2D.IgnoreLayerCollision(3, 8, true);
        yield return new WaitForSeconds(Data.InvincibleTime);
        Physics2D.IgnoreLayerCollision(3, 8, false);
    }
    public void Heal(int amount)
    {
        currentHealthPoints += amount;
        if (currentHealthPoints > maxHealthPoints)
        {
            currentHealthPoints = maxHealthPoints;
        }
        healthBar.SetHealth(currentHealthPoints);
    }

    #region Level Methods
    private void HandleExperienceChanged(int amount)
    {
        currentExperiencePoints += amount;
        if (currentExperiencePoints >= maxExperiencePoints)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExperiencePoints = 0;
        maxExperiencePoints += 10;
        maxHealthPoints += 10;
        currentHealthPoints = maxHealthPoints;
        healthBar.SetMaxHealth(maxHealthPoints);
        Debug.Log("Player leveled up to level " + currentLevel);
    }
    #endregion

    public void Die()
    {
        OnPlayerDeath?.Invoke(this, new PlayerEventArgs(this));

        // #if UNITY_EDITOR
        //         UnityEditor.EditorApplication.isPlaying = false;
        // #else
        //         Application.Quit();
        // #endif
    }

    internal void setmaxHealthPoints()
    {
        currentHealthPoints = maxHealthPoints;
        healthBar.SetHealth(currentHealthPoints);
    }
}
