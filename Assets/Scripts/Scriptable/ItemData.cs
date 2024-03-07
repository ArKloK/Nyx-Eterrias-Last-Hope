using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu][System.Serializable]
public class ItemData : ScriptableObject
{
    public string ItemName;
    public ConditionID ConditionID; 
    public Sprite ItemSprite;
    public string ItemDescription; 
    public int HealthBoost; 
    public int AttackBoost;
    public int SpiritualEnergyBoost; 
}
