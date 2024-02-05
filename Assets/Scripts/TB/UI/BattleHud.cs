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
    public TextMeshProUGUI statusText;
    public HealthBar healthBar;
    private TBPlayer player;
    private TBEnemy enemy;
    public Color psnColor;
    public Color brnColor;
    public Color soakColor;
    private Dictionary<ConditionID, Color> conditionColors;

    void setConditionColors()
    {
        conditionColors = new Dictionary<ConditionID, Color>()
        {
            { ConditionID.Poisoned, psnColor },
            { ConditionID.Burning, brnColor },
            { ConditionID.Soaked, soakColor }
        };
    }

    public void setData(TBEnemy enemy)
    {
        this.enemy = enemy;
        nameText.text = enemy.enemyData.Name;
        levelText.text = "Lvl " + enemy.level;
        healthBar.SetMaxHealth(enemy.MaxHp);
        healthBar.SetHealth(enemy.currentHp);
        SetEnemyStatusText();
        setConditionColors();
        this.enemy.OnStatusChanged += SetEnemyStatusText;
    }

    public void setData(TBPlayer player)
    {
        this.player = player;
        nameText.text = player.playerData.Name;
        levelText.text = "Lvl " + player.level;
        hpText.text = player.currentHp + "/" + player.MaxHp;
        healthBar.SetMaxHealth(player.MaxHp);
        healthBar.SetHealth(player.currentHp);
        SetPlayerStatusText();
        setConditionColors();
        this.player.OnStatusChanged += SetPlayerStatusText;
    }

    public IEnumerator UpdatePlayerHp()
    {
        if (player.hpChanged)
        {
            player.hpChanged = false;
            hpText.text = player.currentHp + "/" + player.MaxHp;
            yield return healthBar.SetHealthAnimated(player.currentHp);
        }
    }

    public IEnumerator UpdateEnemyHp()
    {
        if (enemy.hpChanged)
        {
            enemy.hpChanged = false;
            yield return healthBar.SetHealthAnimated(enemy.currentHp);
        }
    }

    void SetPlayerStatusText()
    {
        if (player.statuses.Count > 0)
        {
            statusText.text = "";
            foreach (var status in player.statuses)
            {
                string colorHex = ColorUtility.ToHtmlStringRGB(conditionColors[status.ID]);
                statusText.text += $"<color=#{colorHex}>{status.HudMessage}</color>\n";
            }
        }
        else
        {
            statusText.text = "";
        }
    }

    void SetEnemyStatusText()
    {
        if (enemy.statuses.Count > 0)
        {
            statusText.text = "";
            foreach (var status in enemy.statuses)
            {
                string colorHex = ColorUtility.ToHtmlStringRGB(conditionColors[status.ID]);
                statusText.text += $"<color=#{colorHex}>{status.HudMessage}</color>\n";
            }
        }
        else
        {
            statusText.text = "";
        }
    }
}
