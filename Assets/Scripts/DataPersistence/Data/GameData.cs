using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    private bool isInitialized;
    public Vector3 playerPosition;
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

    public bool IsEmpty { get => isInitialized; set => isInitialized = value; }

    //The values defined in this constructor will be the default values for the game
    public GameData(PlayerControllerData playerControllerData)
    {
        if(GameObject.Find("FirstRespawnPoint") != null)
        {
            playerPosition = GameObject.Find("FirstRespawnPoint").transform.position;
        }
        else
        {
            playerPosition = new Vector3(0, 0, 0);
        }
        isInitialized = true;
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
        maxExperiencePoints = playerControllerData.BaseMaxExperiencePoints;
        currentLevel = 1;
        collectibles = new SerializableDictionary<string, bool>();
        inventoryItems = new List<InventoryItemSerializable>();
        moves = PlayerStats.GetMovesIndexesLearnedAtLevel(currentLevel);
        if(moves.Length == 0)
        {
            moves = new int[4];
            moves[0] = 0;
            moves[1] = 1;
            moves[2] = 2;
            moves[3] = 3;
        }
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