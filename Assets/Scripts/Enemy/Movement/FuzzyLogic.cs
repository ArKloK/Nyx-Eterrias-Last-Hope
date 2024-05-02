using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuzzyLogic : MonoBehaviour
{
    AnimationCurve playerLowHealth;
    AnimationCurve playerMediumHealth;
    AnimationCurve playerHighHealth;
    AnimationCurve enemyLowHealth;
    AnimationCurve enemyMediumHealth;
    AnimationCurve enemyHighHealth;
    private float[,] rules = new float[3, 3];
    private float LOW, MEDIUM, HIGH;

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
        LOW = 1000; MEDIUM = 1700; HIGH = 3000;
        SetRules();
        setCurvesValues();
    }

    void setCurvesValues()
    {
        //Set all the curves keys to 0
        playerLowHealth = new AnimationCurve();
        playerMediumHealth = new AnimationCurve();
        playerHighHealth = new AnimationCurve();
        enemyLowHealth = new AnimationCurve();
        enemyMediumHealth = new AnimationCurve();
        enemyHighHealth = new AnimationCurve();

        int playerMaxHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().MaxHealthPoints;
        Debug.Log("Player Max Health: " + playerMaxHealth);
        //Set the keys for the player health curves
        playerLowHealth.AddKey(0, 1);
        playerLowHealth.AddKey(playerMaxHealth / 2, 0);
        playerMediumHealth.AddKey(0, 0);
        playerMediumHealth.AddKey(playerMaxHealth / 2, 1);
        playerMediumHealth.AddKey(playerMaxHealth, 0);
        playerHighHealth.AddKey(playerMaxHealth / 2, 0);
        playerHighHealth.AddKey(playerMaxHealth, 1);

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
        rules[0, 0] = LOW;
        rules[0, 1] = LOW;
        rules[0, 2] = MEDIUM;
        rules[1, 0] = LOW;
        rules[1, 1] = MEDIUM;
        rules[1, 2] = HIGH;
        rules[2, 0] = MEDIUM;
        rules[2, 1] = HIGH;
        rules[2, 2] = HIGH;
    }

    private float[] EvaluatePlayerHealth()
    {
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        float[] result = new float[3];
        if (playerController != null)
        {
            int playerHealth = playerController.CurrentHealthPoints;
            result[0] = playerLowHealth.Evaluate(playerHealth);
            result[1] = playerMediumHealth.Evaluate(playerHealth);
            result[2] = playerHighHealth.Evaluate(playerHealth);
        }

        Debug.Log("Player Health: " + result[0] + " " + result[1] + " " + result[2]);
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
        float[] playerHealthEvaluation = EvaluatePlayerHealth();


        int x = 0, y = 0;
        float num = 0, den = 0;


        for (int i = 0; i < 9; i++)
        {
            //Debug.Log(Mathf.Min(healthEvaluation[x], ammoEvaluation[y]));
            num += rules[x, y] * Mathf.Min(enemyHealthEvaluation[x], playerHealthEvaluation[y]);
            den += Mathf.Min(enemyHealthEvaluation[x], playerHealthEvaluation[y]);
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
