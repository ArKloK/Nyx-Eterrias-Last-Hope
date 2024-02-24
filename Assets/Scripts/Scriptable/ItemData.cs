using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    [SerializeField] string itemName;
    [SerializeField] Sprite itemSprite;
    [SerializeField] string itemDescription;

    [Header("Stats Boost")]
    [SerializeField] int healthBoost;
    [SerializeField] int attackBoost;
    [SerializeField] int spiritualEnergyBoost;
    [SerializeField] ConditionID conditionID;

    public string ItemName { get => itemName; set => itemName = value; }
    public ConditionID ConditionID { get => conditionID; set => conditionID = value; }
    public Sprite ItemSprite { get => itemSprite; set => itemSprite = value; }
    public string ItemDescription { get => itemDescription; set => itemDescription = value; }

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
