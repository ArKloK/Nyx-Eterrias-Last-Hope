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

    //The values defined in this constructor will be the default values for the game
    public GameData(PlayerControllerData playerControllerData)
    {
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
        moves = PlayerStats.GetMovesIndexesLearnedAtLevel(currentLevel);
    }

}