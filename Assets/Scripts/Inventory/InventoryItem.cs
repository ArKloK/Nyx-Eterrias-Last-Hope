using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem
{
    public ItemData itemData;
    public int quantity;

    public InventoryItem(ItemData itemData)
    {
        this.itemData = itemData;
        AddQuantity();
    }

    public void AddQuantity()
    {
        quantity ++;
    }

    public void RemoveQuantity()
    {
        quantity --;
    }
}
