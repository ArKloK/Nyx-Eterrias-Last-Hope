using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static event Action <List<InventoryItem>> OnInventoryChange;
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();
    private Dictionary<ItemData, InventoryItem> inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

    void OnEnable()
    {
        Potion.OnPotionCollected += AddItem;
        Coin.OnCoinCollected += AddItem;
        InventoryManager.OnItemUsed += RemoveItem;
    }

    void OnDisable()
    {
        Potion.OnPotionCollected -= AddItem;
        Coin.OnCoinCollected -= AddItem;
        InventoryManager.OnItemUsed -= RemoveItem;
    }

    public void AddItem(ItemData itemData)
    {
        // If the item is already in the inventory, increase the quantity
        if (inventoryDictionary.TryGetValue(itemData, out InventoryItem item))
        {
            item.AddQuantity();
            Debug.Log($"{itemData.itemName} total stack is now {item.Quantity}");
            OnInventoryChange?.Invoke(inventoryItems);
        }
        // If the item is not in the inventory, add it
        else
        {
            InventoryItem newInventoryItem = new InventoryItem(itemData);
            inventoryItems.Add(newInventoryItem);
            inventoryDictionary.Add(itemData, newInventoryItem);
            Debug.Log($"Added {itemData.itemName} to inventory for the first time.");
            OnInventoryChange?.Invoke(inventoryItems);
        }
    }
    public void RemoveItem(ItemData itemData)
    {
        if (inventoryDictionary.TryGetValue(itemData, out InventoryItem item))
        {
            item.RemoveQuantity();
            if (item.Quantity <= 0)
            {
                inventoryItems.Remove(item);
                inventoryDictionary.Remove(itemData);
            }
            Debug.Log($"Removed a unit of {itemData.itemName}");
            OnInventoryChange?.Invoke(inventoryItems);
        }
    }

    public void LaunchInventoryChange()
    {
        OnInventoryChange?.Invoke(inventoryItems);
    }
}
