using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TBEnemy
{
    public TBEnemyData enemyData;
    public int currentHp;
    public int level;

    public TBEnemy(TBEnemyData enemyData, int level)
    {
        this.enemyData = enemyData;
        this.level = level;
        currentHp = enemyData.MaxHealthPoints;
    }

    public int Attack
    {
        get
        {
            return enemyData.AttackPower * level;
        }
    }

    public int Defense
    {
        get
        {
            return enemyData.DefensePower * level;
        }
    }

    public int Speed
    {
        get
        {
            return enemyData.AttackSpeed * level;
        }
    }

    public int MaxHp
    {
        get
        {
            return enemyData.MaxHealthPoints * level;
        }
    }

}
