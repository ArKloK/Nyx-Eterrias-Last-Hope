using UnityEngine;

public class FuzzyLogic : MonoBehaviour
{
    AnimationCurve playerLowLevel;
    AnimationCurve playerMediumLevel;
    AnimationCurve playerHighLevel;
    AnimationCurve enemyLowHealth;
    AnimationCurve enemyMediumHealth;
    AnimationCurve enemyHighHealth;
    private float[,] rules = new float[3, 3];
    private float SLOW, MEDIUM, FAST;

    void OnEnable()
    {
        PlayerController.OnPlayerLevelUp += setCurvesValues;
    }

    void OnDisable()
    {
        PlayerController.OnPlayerLevelUp -= setCurvesValues;
    }

    void Start()
    {
        SLOW = 1200; MEDIUM = 2200; FAST = 3200;
        SetRules();
        setCurvesValues();
    }

    void setCurvesValues()
    {
        //Set all the curves keys to 0
        playerLowLevel = new AnimationCurve();
        playerMediumLevel = new AnimationCurve();
        playerHighLevel = new AnimationCurve();
        enemyLowHealth = new AnimationCurve();
        enemyMediumHealth = new AnimationCurve();
        enemyHighHealth = new AnimationCurve();

        int playerMaxLevel = 10;
        //Set the keys for the player health curves
        playerLowLevel.AddKey(0, 1);
        playerLowLevel.AddKey(playerMaxLevel / 2, 0);
        playerMediumLevel.AddKey(0, 0);
        playerMediumLevel.AddKey(playerMaxLevel / 2, 1);
        playerMediumLevel.AddKey(playerMaxLevel, 0);
        playerHighLevel.AddKey(playerMaxLevel / 2, 0);
        playerHighLevel.AddKey(playerMaxLevel, 1);

        //Set the keys for the enemy health curves
        enemyLowHealth.AddKey(0, 1);
        enemyLowHealth.AddKey(GetComponent<EnemyController>().Data.MaxHealthPoints / 2, 0);
        enemyMediumHealth.AddKey(0, 0);
        enemyMediumHealth.AddKey(GetComponent<EnemyController>().Data.MaxHealthPoints / 2, 1);
        enemyMediumHealth.AddKey(GetComponent<EnemyController>().Data.MaxHealthPoints, 0);
        enemyHighHealth.AddKey(GetComponent<EnemyController>().Data.MaxHealthPoints / 2, 0);
        enemyHighHealth.AddKey(GetComponent<EnemyController>().Data.MaxHealthPoints, 1);
    }

    void SetRules()
    {
        rules[0, 0] = SLOW;
        rules[0, 1] = SLOW;
        rules[0, 2] = MEDIUM;
        rules[1, 0] = SLOW;
        rules[1, 1] = MEDIUM;
        rules[1, 2] = FAST;
        rules[2, 0] = MEDIUM;
        rules[2, 1] = FAST;
        rules[2, 2] = FAST;
    }

    private float[] EvaluatePlayerLevel()
    {
        PlayerController playerController = null;
        if(GameObject.FindGameObjectWithTag("Player") != null)
        {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }
        
        float[] result = new float[3];
        if (playerController != null)
        {
            int playerLevel = playerController.CurrentLevel;
            result[0] = playerLowLevel.Evaluate(playerLevel);
            result[1] = playerMediumLevel.Evaluate(playerLevel);
            result[2] = playerHighLevel.Evaluate(playerLevel);
        }

        Debug.Log("Player Level: " + result[0] + " " + result[1] + " " + result[2]);
        return result;
    }

    private float[] EvaluateEnemyHealth()
    {
        EnemyController enemyController = GetComponent<EnemyController>();
        float[] result = new float[3];
        if (enemyController != null)
        {
            int enemyHealth = enemyController.CurrentHealthPoints;
            result[0] = enemyLowHealth.Evaluate(enemyHealth);
            result[1] = enemyMediumHealth.Evaluate(enemyHealth);
            result[2] = enemyHighHealth.Evaluate(enemyHealth);
        }

        Debug.Log("Enemy Health: " + result[0] + " " + result[1] + " " + result[2]);
        return result;
    }

    public float SetEnemySpeed()
    {
        float[] enemyHealthEvaluation = EvaluateEnemyHealth();
        float[] playerLevelEvaluation = EvaluatePlayerLevel();


        int x = 0, y = 0;
        float num = 0, den = 0;


        for (int i = 0; i < 9; i++)
        {
            //Debug.Log(Mathf.Min(healthEvaluation[x], ammoEvaluation[y]));
            num += rules[x, y] * Mathf.Min(enemyHealthEvaluation[x], playerLevelEvaluation[y]);
            den += Mathf.Min(enemyHealthEvaluation[x], playerLevelEvaluation[y]);
            y++;
            if (y % 3 == 0)
            {
                y = 0;
                x++;
            }
        }

        return num / den;
    }
}
