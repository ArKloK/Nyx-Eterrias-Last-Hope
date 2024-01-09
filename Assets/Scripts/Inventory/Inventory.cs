using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();
    private Dictionary<ItemData, InventoryItem> inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

    void OnEnable()
    {
        Coin.OnCoinCollected += AddItem;
    }

    void OnDisable()
    {
        Coin.OnCoinCollected -= AddItem;
    }

    public void AddItem(ItemData itemData)
    {
        if (inventoryDictionary.TryGetValue(itemData, out InventoryItem item))
        {
            item.AddQuantity();
            Debug.Log($"{itemData.itemName} total stack is now {item.quantity}");
        }
        else
        {
            InventoryItem newInventoryItem = new InventoryItem(itemData);
            inventoryItems.Add(newInventoryItem);
            inventoryDictionary.Add(itemData, newInventoryItem);
            Debug.Log($"Added {itemData.itemName} to inventory for the first time.");
        }
    }

    public void RemoveItem(ItemData itemData)
    {
        if (inventoryDictionary.TryGetValue(itemData, out InventoryItem item))
        {
            item.RemoveQuantity();
            if (item.quantity <= 0)
            {
                inventoryItems.Remove(item);
                inventoryDictionary.Remove(itemData);
            }
        }
    }
}
