using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI hpText;
    public HealthBar healthBar;
    private TBPlayer player;
    private TBEnemy enemy;

    public void setData(TBEnemy enemy)
    {
        this.enemy = enemy;
        nameText.text = enemy.enemyData.Name;
        levelText.text = "Lvl " + enemy.level;
        healthBar.SetMaxHealth(enemy.MaxHp);
        healthBar.SetHealth(enemy.currentHp);
    }

    public void setData(TBPlayer player)
    {
        this.player = player;
        nameText.text = player.playerData.Name;
        levelText.text = "Lvl " + player.level;
        hpText.text = player.currentHp + "/" + player.MaxHp;
        healthBar.SetMaxHealth(player.MaxHp);
        healthBar.SetHealth(player.currentHp);
    }

    public IEnumerator UPdatePlayerHp()
    {
        hpText.text = player.currentHp + "/" + player.MaxHp;
        yield return healthBar.SetHealthAnimated(player.currentHp);
    }

    public IEnumerator UpdateEnemyHp()
    {
        yield return healthBar.SetHealthAnimated(enemy.currentHp);
    }

    // public void UpdatePlayerHp()
    // {
    //     hpText.text = player.currentHp + "/" + player.MaxHp;
    //     healthBar.SetHealth(player.currentHp);
    // }

    // public void UpdateEnemyHp()
    // {
    //     healthBar.SetHealth(enemy.currentHp);
    // }
}
