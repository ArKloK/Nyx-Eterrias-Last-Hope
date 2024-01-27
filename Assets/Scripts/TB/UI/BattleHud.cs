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
        healthBar.SetMaxHealth(enemy.MaxHp);
        healthBar.SetHealth(enemy.currentHp);
    }

    public void setData(TBPlayer player)
    {
        nameText.text = player.playerData.Name;
        levelText.text = "Lvl " + player.level;
        healthBar.SetMaxHealth(player.MaxHp);
        healthBar.SetHealth(player.currentHp);
        Debug.Log("Player Max HP: " + player.MaxHp);
        Debug.Log("Player HP: " + player.currentHp);
    }
}
