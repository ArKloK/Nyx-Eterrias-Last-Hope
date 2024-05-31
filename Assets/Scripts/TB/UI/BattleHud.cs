using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleHud : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI elementText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI statusText;
    public HealthBar healthBar;
    private TBCharacter character;
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

    public void SetData(TBCharacter ch)
    {
        character = ch;
        nameText.text = character.CharacterData.Name;
        levelText.text = "Lvl " + character.Level;
        if(!character.CharacterData.IsEnemy) hpText.text = character.CurrentHP + "/" + character.MaxHp;
            
        elementText.text = character.CharacterData.Element.ToString();
        switch (character.CharacterData.Element)
        {
            case Element.Fire:
                elementText.color = Color.red;
                break;
            case Element.Water:
                elementText.color = Color.blue;
                break;
            case Element.Grass:
                elementText.color = Color.green;
                break;
        }
        healthBar.SetMaxHealth(character.MaxHp);
        healthBar.SetHealth(character.CurrentHP);
        SetStatusText();
        setConditionColors();
        this.character.OnStatusChanged += SetStatusText;
    }

    public IEnumerator UpdateHp()
    {
        if (character.HpChanged)
        {
            character.HpChanged = false;
            if (!character.CharacterData.IsEnemy)
                hpText.text = character.CurrentHP + "/" + character.MaxHp;
            yield return healthBar.SetHealthAnimated(character.CurrentHP);
        }
    }

    void SetStatusText()
    {
        if (character.Statuses.Count > 0)
        {
            statusText.text = "";
            foreach (var status in character.Statuses)
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
