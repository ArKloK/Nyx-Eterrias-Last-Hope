using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int element;
    public int maxHealthPoints;
    public int currentHealthPoints;
    public int maxSpiritualEnergyPoints;
    public int currentSpiritualEnergyPoints;
    public int attackPower;
    public int tBAttackSpeed;
    public int tBDefensePower;
    public int tBAttackPower;
    public int currentExperiencePoints;
    public int maxExperiencePoints;
    public int currentLevel;
    public int[] moves;
    public SerializableDictionary<string, bool> collectibles;
    public List<InventoryItemSerializable> inventoryItems;

    //The values defined in this constructor will be the default values for the game
    public GameData(PlayerControllerData playerControllerData)
    {
        //if (playerControllerData == null) return;
        element = (int)playerControllerData.Element;
        maxHealthPoints = playerControllerData.MaxHealthPoints;
        currentHealthPoints = maxHealthPoints;
        maxSpiritualEnergyPoints = playerControllerData.MaxSpiritualEnergyPoints;
        currentSpiritualEnergyPoints = maxSpiritualEnergyPoints;
        attackPower = playerControllerData.AttackPower;
        tBAttackSpeed = playerControllerData.TBAttackSpeed;
        tBDefensePower = playerControllerData.TBDefensePower;
        tBAttackPower = playerControllerData.TBAttackPower;
        currentExperiencePoints = 0;
        maxExperiencePoints = playerControllerData.MaxExperiencePoints;
        currentLevel = 1;
        collectibles = new SerializableDictionary<string, bool>();
        inventoryItems = new List<InventoryItemSerializable>();
        moves = PlayerStats.GetMovesIndexesLearnedAtLevel(currentLevel);
    }
    
    public override string ToString()
    {
        return "Element: " + element + "\n" +
            "Max Health Points: " + maxHealthPoints + "\n" +
            "Current Health Points: " + currentHealthPoints + "\n" +
            "Max Spiritual Energy Points: " + maxSpiritualEnergyPoints + "\n" +
            "Current Spiritual Energy Points: " + currentSpiritualEnergyPoints + "\n" +
            "Attack Power: " + attackPower + "\n" +
            "TB Attack Speed: " + tBAttackSpeed + "\n" +
            "TB Defense Power: " + tBDefensePower + "\n" +
            "TB Attack Power: " + tBAttackPower + "\n" +
            "Current Experience Points: " + currentExperiencePoints + "\n" +
            "Max Experience Points: " + maxExperiencePoints + "\n" +
            "Current Level: " + currentLevel + "\n" +
            "Collectibles: " + collectibles + "\n" +
            "Inventory Items: " + inventoryItems + "\n" +
            "Moves: " + moves + "\n";
    }

}