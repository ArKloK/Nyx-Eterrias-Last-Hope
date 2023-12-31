using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerControllerData Data;
    #region Health and Energy Variables
    private int maxHealthPoints;
    private int currentHealthPoints;
    private int currentSpiritualEnergyPoints;
    #endregion
    #region Level Variables
    private int currentExperiencePoints;
    private int maxExperiencePoints;
    private int currentLevel;
    #endregion

    void Awake()
    {
        maxHealthPoints = Data.MaxHealthPoints;
        currentHealthPoints = maxHealthPoints;
        currentSpiritualEnergyPoints = Data.MaxSpiritualEnergyPoints;
    }

    void Start()
    {
        currentExperiencePoints = 0;
        maxExperiencePoints = Data.MaxExperiencePoints;
        currentLevel = 1;
    }

    void OnEnable()
    {
        //Singleton check
        if (ExperienceManager.Instance == null)
        {
            ExperienceManager.Instance = new GameObject("ExperienceManager").AddComponent<ExperienceManager>();
        }
        ExperienceManager.Instance.OnExperienceChanged += HandleExperienceChanged;
    }

    void OnDisable()
    {
        ExperienceManager.Instance.OnExperienceChanged -= HandleExperienceChanged;
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
        if (currentHealthPoints <= 0)
        {
            Die();
        }
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
        Debug.Log("Player leveled up to level " + currentLevel);
    }
    #endregion

    public void Die()
    {
        this.gameObject.SetActive(false);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
