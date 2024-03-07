using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItemSerializable
{
    public ItemDataSerializable ItemDataSerializable;
    public int Quantity;

    public InventoryItemSerializable(InventoryItem inventoryItem)
    {
        ItemDataSerializable = new ItemDataSerializable(inventoryItem.ItemData);
        Quantity = inventoryItem.Quantity;
    }
}
