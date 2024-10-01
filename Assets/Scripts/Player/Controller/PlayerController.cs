using System;
using System.Collections;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class PlayerEventArgs : EventArgs
{
    public PlayerController PlayerController { get; set; }

    public PlayerEventArgs(PlayerController player)
    {
        PlayerController = player;
    }
}

public class PlayerController : MonoBehaviour, IDataPersistence
{
    [Header("Script References")]
    [SerializeField] PlayerControllerData Data;
    [SerializeField] HealthBar healthBar;
    [SerializeField] ExperienceBar experienceBar;
    private PlayerMovementController.PlayerMovementController playerMovementController;
    [SerializeField] TBCharacterData tBCharacterData;

    #region Health and Energy Variables
    private int maxHealthPoints;
    private int currentHealthPoints;
    public int CurrentHealthPoints { get => currentHealthPoints; set => currentHealthPoints = value; }
    public int MaxHealthPoints { get => maxHealthPoints; set => maxHealthPoints = value; }
    public int CurrentLevel { get => currentLevel; set => currentLevel = value; }

    private int maxSpriritualEnergyPoints;
    private int currentSpiritualEnergyPoints;
    #endregion

    #region Attack Variables
    private int attackPower;
    private float attackRadius;
    [SerializeField] Transform attackOrigin;
    private LayerMask enemyMask;
    #endregion

    #region TBCombat Variables
    private int TBattackPower;
    private int TBdefensePower;
    private int TBattackSpeed;
    [SerializeField] bool TBDemo;
    #endregion

    #region Level Variables
    private int currentExperiencePoints;
    private int maxExperiencePoints;
    private int currentLevel;
    [SerializeField] TextMeshProUGUI levelText;
    #endregion

    #region Events
    public static event EventHandler<PlayerEventArgs> OnPlayerDeath;
    public static event Action OnPlayerLevelUp;
    #endregion

    void Awake()
    {
        PlayerStats.Element = Data.Element;
        PlayerStats.LearnableMoves = tBCharacterData.LearnableMoves;
        maxHealthPoints = Data.MaxHealthPoints;
        currentHealthPoints = maxHealthPoints;
        maxSpriritualEnergyPoints = Data.MaxSpiritualEnergyPoints;
        currentSpiritualEnergyPoints = maxSpriritualEnergyPoints;
        attackPower = Data.AttackPower;
        TBattackPower = Data.TBAttackPower;
        TBdefensePower = Data.TBDefensePower;
        TBattackSpeed = Data.TBAttackSpeed;
        currentExperiencePoints = 0;
        maxExperiencePoints = Data.BaseMaxExperiencePoints;
        currentLevel = 1;

        if (!TBDemo)
        {
            healthBar.SetMaxHealth(maxHealthPoints);
            experienceBar.SetMaxExperience(maxExperiencePoints);
            experienceBar.SetExperience(currentExperiencePoints);
            levelText.text = currentLevel.ToString();
        }
    }
    void Start()
    {
        playerMovementController = GetComponent<PlayerMovementController.PlayerMovementController>();

        attackPower = Data.AttackPower;
        attackRadius = Data.AttackRadius;
        enemyMask = Data.EnemyMask;

        UpdateStaticStats();//This is called here as well to update the static stats before the method PlayerStats.SetMoves() is called

        PlayerStats.SetMoves();
    }

    void OnEnable()
    {
        ExperienceManager.OnExperienceChanged += HandleExperienceChanged;
        SelectMovementAgent.OnEpisodeBeginAction += UpdateStaticStats;
    }

    void OnDisable()
    {
        ExperienceManager.OnExperienceChanged -= HandleExperienceChanged;
        SelectMovementAgent.OnEpisodeBeginAction -= UpdateStaticStats;
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage();
            Debug.Log("Player took damage, current health: " + currentHealthPoints);
        }
        if (!TBDemo) UpdateStaticStats();
    }

    public void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackOrigin.position, attackRadius, enemyMask);
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.TakeDamage(attackPower, transform.position);
                Debug.Log("Enemy took damage, current health: " + enemyController.CurrentHealthPoints);
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
    public void TakeDamage()
    {
        float damage = Mathf.Ceil(maxHealthPoints * 0.1f);
        Debug.Log("Player took damage: " + damage);
        currentHealthPoints -= (int)damage;
        healthBar.SetHealth(currentHealthPoints);
        if (currentHealthPoints <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(Vector2 knockbackDirection)
    {
        float damage = Mathf.Ceil(maxHealthPoints * 0.1f);
        Debug.Log("Player took damage: " + damage);
        currentHealthPoints -= (int)damage;
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
        int leftoverExp = amount;

        while (leftoverExp > 0)
        {
            Debug.Log("Amount: " + amount);
            int experienceNeeded = maxExperiencePoints - currentExperiencePoints;
            Debug.Log("Experience needed: " + experienceNeeded);

            if (leftoverExp >= experienceNeeded)
            {
                leftoverExp -= experienceNeeded;
                LevelUp();
            }
            else
            {
                currentExperiencePoints += leftoverExp;
                leftoverExp = 0;
            }
        }

        experienceBar.SetExperience(currentExperiencePoints);
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExperiencePoints = 0;
        maxExperiencePoints += 10;
        maxHealthPoints = Data.MaxHealthPoints * currentLevel;
        currentHealthPoints = maxHealthPoints;
        currentSpiritualEnergyPoints = maxSpriritualEnergyPoints;
        UpdateStaticStats();//Updates the static stats when the player levels up
        healthBar.SetMaxHealth(maxHealthPoints);
        experienceBar.SetMaxExperience(maxExperiencePoints);
        experienceBar.SetExperience(currentExperiencePoints);
        levelText.text = currentLevel.ToString();
        OnPlayerLevelUp?.Invoke();
        Debug.Log("Current HP: " + currentHealthPoints);
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

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackOrigin.position, attackRadius);
    }

    internal void setmaxHealthPoints()
    {
        currentHealthPoints = maxHealthPoints;
        healthBar.SetHealth(currentHealthPoints);
    }

    void UpdateStaticStats()
    {
        PlayerStats.Element = Data.Element;
        PlayerStats.MaxHealthPoints = maxHealthPoints;
        PlayerStats.CurrentHealthPoints = currentHealthPoints;
        PlayerStats.MaxSpiritualEnergyPoints = maxSpriritualEnergyPoints;
        PlayerStats.CurrentSpiritualEnergyPoints = currentSpiritualEnergyPoints;
        PlayerStats.TBAttackPower = TBattackPower;
        PlayerStats.AttackPower = attackPower;
        PlayerStats.TBAttackSpeed = TBattackSpeed;
        PlayerStats.TBDefensePower = TBdefensePower;
        PlayerStats.CurrentExperiencePoints = currentExperiencePoints;
        PlayerStats.MaxExperiencePoints = maxExperiencePoints;
        PlayerStats.CurrentLevel = currentLevel;
    }

    public void SetLocalStats()
    {
        maxHealthPoints = PlayerStats.MaxHealthPoints;
        currentHealthPoints = PlayerStats.CurrentHealthPoints;
        TBattackPower = PlayerStats.TBAttackPower;
        attackPower = PlayerStats.AttackPower;
        TBattackSpeed = PlayerStats.TBAttackSpeed;
        TBdefensePower = PlayerStats.TBDefensePower;
        currentExperiencePoints = PlayerStats.CurrentExperiencePoints;
        maxExperiencePoints = PlayerStats.MaxExperiencePoints;
        currentLevel = PlayerStats.CurrentLevel;
        maxSpriritualEnergyPoints = PlayerStats.MaxSpiritualEnergyPoints;
        currentSpiritualEnergyPoints = PlayerStats.CurrentSpiritualEnergyPoints;
        if (!TBDemo)
        {
            healthBar.SetHealth(currentHealthPoints);
            experienceBar.SetExperience(currentExperiencePoints);
        }
    }

    public void LoadData(GameData gameData)
    {
        transform.position = gameData.playerPosition;
        PlayerStats.StaticLoadData(gameData);
        SetLocalStats();
    }

    public void SaveData(GameData gameData)
    {
        gameData.playerPosition = transform.position;
        PlayerStats.StaticSaveData(gameData);
    }
}
