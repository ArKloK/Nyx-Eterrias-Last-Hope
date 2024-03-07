using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDataSerializable
{
    public string ItemName;
    public string ItemSpriteName;
    public string ItemDescription;
    public int HealthBoost;
    public int AttackBoost;
    public int SpiritualEnergyBoost;
    public int ConditionID;

    public ItemDataSerializable(ItemData itemData)
    {
        //if (itemData == null) return;
        ItemName = itemData.ItemName;
        ItemSpriteName = itemData.ItemSprite.name;
        ItemDescription = itemData.ItemDescription;
        HealthBoost = itemData.HealthBoost;
        AttackBoost = itemData.AttackBoost;
        SpiritualEnergyBoost = itemData.SpiritualEnergyBoost;
        ConditionID = (int)itemData.ConditionID;
    }
}
