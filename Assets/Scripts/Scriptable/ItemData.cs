using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public string itemDescription;

    [Header("Stats Boost")]
    public int healthBoost;
    public int attackBoost;
    public int spiritualEnergyBoost;

    public void UseItem()
    {
        Debug.Log("Using " + itemName);
        PlayerStats.CurrentHealthPoints += healthBoost;
        PlayerStats.AttackPower += attackBoost;
        PlayerStats.CurrentSpiritualEnergyPoints += spiritualEnergyBoost;
        FindObjectOfType<PlayerController>().setStats();
        TBCharacterUnit[] tBCharacterUnit = FindObjectsOfType<TBCharacterUnit>();
        Debug.Log("TBCharacterUnit: " + tBCharacterUnit);
        foreach (TBCharacterUnit tBCharacter in tBCharacterUnit)
        {
            if (tBCharacter != null && !tBCharacter.Character.CharacterData.IsEnemy)
            {
                Debug.Log("Setting stats");
                tBCharacter.Character.setStats();
            }
        }
    }
}
