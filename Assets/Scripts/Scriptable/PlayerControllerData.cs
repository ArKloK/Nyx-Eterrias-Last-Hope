using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu][System.Serializable]
public class PlayerControllerData : ScriptableObject
{
    [Header("PLAYER ELEMENT")]
    [Tooltip("The element of the player")]
    public Element Element;

    [Header("HEALTH AND ENERGY")]
    [Tooltip("The maximum health for the player at level 1, this will increase as the player levels up")]
    public int MaxHealthPoints;
    [Tooltip("The maximum spiritual energy for the player")]
    public int MaxSpiritualEnergyPoints;

    [Header("ATTACK")]
    [Tooltip("The amount of damage the player does")]
    public int AttackPower;
    [Tooltip("The speed of the player's attack")]
    public float AttackSpeed;
    [Tooltip("The radius of the player's attack")]
    public float AttackRadius;
    [Tooltip("The layer mask for the enemies")]
    public LayerMask EnemyMask;

    [Header("LEVEL STATS")]
    [Tooltip("The maximum experience the player needs to level up, this will increase as the player levels up")]
    public int BaseMaxExperiencePoints;

    [Header("KNOCKBACK DATA")]
    [Tooltip("The amount of time the player can't move after being hit")]
    public float LoseControlTime;
    [Tooltip("The amount of time the player is invincible after being hit")]
    public float InvincibleTime;

    [Header("TURN BASED COMBAT")]
    [Tooltip("The amount of damage the player does in the TB combat")]
    public int TBAttackPower;
    [Tooltip("The speed of the player's attack in the TB combat")]
    public int TBAttackSpeed;
    [Tooltip("The amount of damage the player can take")]
    public int TBDefensePower;

}

public enum Element
{

    Normal,
    Fire,
    Water,
    Grass,
    Darkness,
}

public enum Stat
{
    Attack,
    Defense,
    Speed,
    Accuracy,
}

public class TypeChart
{
    public static float[][] chart =
    {
        //HAS TO BE IMPROVED
        //           Fire, Water, Plant, Darkness
        new float[] {1f,   0.5f,  1.5f,    1f}, //Fire
        new float[] {1.5f,   1f,    0.5f,  1f}, //Water
        new float[] {0.5f, 1.5f,    1f,    1f}, //Plant
        new float[] {1f,   1f,    1f,    1f}, //Darkness
    };

    public static float GetEffectiveness(Element attack, Element defense)
    {
        if (attack == Element.Normal || defense == Element.Normal)
            return 1f;
        return chart[(int)attack - 1][(int)defense - 1];
    }
}
