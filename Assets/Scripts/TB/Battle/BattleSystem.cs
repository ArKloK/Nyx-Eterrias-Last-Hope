using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public EnemyUnit enemyUnit;
    public BattleHud enemyHud;

    void Start()
    {
        enemyUnit.setData();
        enemyHud.setData(enemyUnit.enemy);
    }
}
