using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuzzyLogic : MonoBehaviour
{
    public AnimationCurve playerLowHealth;
    public AnimationCurve playerMediumHealth;
    public AnimationCurve playerHighHealth;
    public AnimationCurve enemyLowHealth;
    public AnimationCurve enemyMediumHealth;
    public AnimationCurve enemyHighHealth;
    private float[,] rules = new float[3, 3];
    private float LOW, MEDIUM, HIGH;

    void setCurvesValues()
    {
        //Set all the curves keys to 0
        playerLowHealth = new AnimationCurve();
        playerMediumHealth = new AnimationCurve();
        playerHighHealth = new AnimationCurve();
        enemyLowHealth = new AnimationCurve();
        enemyMediumHealth = new AnimationCurve();
        enemyHighHealth = new AnimationCurve();

        //Set the keys for the player health curves
        playerLowHealth.AddKey(0, 1);
        playerLowHealth.AddKey(PlayerStats.MaxHealthPoints/2, 0);
        playerMediumHealth.AddKey(0, 0);
        playerMediumHealth.AddKey(PlayerStats.MaxHealthPoints/2, 1);
        playerMediumHealth.AddKey(PlayerStats.MaxHealthPoints, 0);
        playerHighHealth.AddKey(PlayerStats.MaxHealthPoints/2, 0);
        playerHighHealth.AddKey(PlayerStats.MaxHealthPoints, 1);

        //Set the keys for the enemy health curves
        enemyLowHealth.AddKey(0, 1);
        enemyLowHealth.AddKey(GetComponent<EnemyController>().Data.MaxHealthPoints/2, 0);
        enemyMediumHealth.AddKey(0, 0);
        enemyMediumHealth.AddKey(GetComponent<EnemyController>().Data.MaxHealthPoints/2, 1);
        enemyMediumHealth.AddKey(GetComponent<EnemyController>().Data.MaxHealthPoints, 0);
        enemyHighHealth.AddKey(GetComponent<EnemyController>().Data.MaxHealthPoints/2, 0);
        enemyHighHealth.AddKey(GetComponent<EnemyController>().Data.MaxHealthPoints, 1);
    }

    void Start()
    {
        LOW = 1400; MEDIUM = 2000; HIGH = 3000;
        SetRules();
        setCurvesValues();
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

        return result;
    }

    private void setZombieSpeed()
    {
        float[] enemyHealthEvaluation = EvaluateEnemyHealth();
        float[] playerHealthEvaluation = EvaluatePlayerHealth();


        int x = 0, y = 0;
        float num = 0, den = 0;


        for (int i = 0; i < 9; i++)
        {
            //Debug.Log(Mathf.Min(healthEvaluation[x], ammoEvaluation[y]));
            num += rules[x, y] * Mathf.Min(playerHealthEvaluation[x], enemyHealthEvaluation[y]);
            den += Mathf.Min(playerHealthEvaluation[x], enemyHealthEvaluation[y]);
            y++;
            if (y % 3 == 0)
            {
                y = 0;
                x++;
            }
        }

        Debug.Log(num / den);
        GetComponent<EnemyAI>().Speed = num / den;
    }
}
