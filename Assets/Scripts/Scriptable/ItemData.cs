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
        int currentHealthPoints, currentSpiritualEnergyPoints;
        currentHealthPoints = PlayerStats.CurrentHealthPoints += healthBoost;
        currentSpiritualEnergyPoints = PlayerStats.CurrentSpiritualEnergyPoints += spiritualEnergyBoost;
        Debug.Log("Using " + itemName);

        // Clamp the health and spiritual energy points to their maximum values in case a max boost potion is used
        PlayerStats.CurrentHealthPoints = Mathf.Clamp(currentHealthPoints, 0, PlayerStats.MaxHealthPoints);
        PlayerStats.CurrentSpiritualEnergyPoints = Mathf.Clamp(currentSpiritualEnergyPoints, 0, PlayerStats.MaxSpiritualEnergyPoints);
        //--------------------------------
        
        PlayerStats.AttackPower += attackBoost;
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
