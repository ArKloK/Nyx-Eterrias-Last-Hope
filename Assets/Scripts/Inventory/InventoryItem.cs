using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem
{
    private ItemData itemData;
    private int quantity;

    public ItemData ItemData { get => itemData; set => itemData = value; }
    public int Quantity { get => quantity; set => quantity = value; }

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
