using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public HealthBar healthBar;

    public void setData(TBEnemy enemy)
    {
        nameText.text = enemy.enemyData.EnemyName;
        levelText.text = "Lvl " + enemy.level;
        healthBar.SetHealth(enemy.currentHp);
    }

    public void setData(TBPlayer player)
    {
        nameText.text = player.playerData.Name;
        levelText.text = "Lvl " + player.level;
        healthBar.SetHealth(player.currentHp);
    }
}
