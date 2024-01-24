using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    public TBEnemyData enemyData;
    public int level;
    public TBEnemy enemy;

    public void setData()
    {
        enemy = new TBEnemy(enemyData, level);
    }
}
