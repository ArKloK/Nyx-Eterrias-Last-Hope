using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerControllerData : ScriptableObject
{
    [Header("HEALTH AND ENERGY")]
    [Tooltip("The maximum health for the player at level 1, this will increase as the player levels up")]
    public int MaxHealthPoints;
    [Tooltip("The maximum spiritual energy for the player")]
    public int MaxSpiritualEnergyPoints;

    [Header("ATTACK")]
    [Tooltip("The amount of damage the player does")]
    public int AttackPower;

    [Header("LEVEL STATS")]
    [Tooltip("The maximum experience the player needs to level up, this will increase as the player levels up")]
    public int MaxExperiencePoints;

    [Header("KNOCKBACK DATA")]
    [Tooltip("The amount of time the player can't move after being hit")]
    public float LoseControlTime;
    [Tooltip("The amount of time the player is invincible after being hit")]
    public float InvincibleTime;

    [Header("TURN BASED COMBAT")]
    [Tooltip("The speed of the player's attack in the TB combat")]
    public float AttackSpeed;
    [Tooltip("The amount of damage the player can take")]
    public float DefensePower;

}
