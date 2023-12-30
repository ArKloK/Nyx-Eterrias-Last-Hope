using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyData : ScriptableObject
{
    [Header("HEALTH")]
    [Tooltip("The maximum health for the enemy")]
    public int MaxHealthPoints;

    [Header("EXPERIENCE")]
    [Tooltip("The amount of experience the player gets for killing this enemy")]
    public int ExperiencePoints;
}
