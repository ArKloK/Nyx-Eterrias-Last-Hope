using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour, IDataPersistence
{
    public static event Action<List<InventoryItem>> OnInventoryChange;
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();
    private Dictionary<ItemData, InventoryItem> inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

    void OnEnable()
    {
        Collectible.OnCollectibleCollected += AddItem;
        InventoryManager.OnItemUsed += RemoveItem;
    }

    void OnDisable()
    {
        Collectible.OnCollectibleCollected -= AddItem;
        InventoryManager.OnItemUsed -= RemoveItem;
    }

    public void AddItem(ItemData itemData)
    {
        bool itemExists = inventoryDictionary.TryGetValue(itemData, out InventoryItem item);
        foreach(KeyValuePair<ItemData, InventoryItem> entry in inventoryDictionary)
        {
            if (entry.Key.ItemName == itemData.ItemName)
            {
                itemData = entry.Key;//This will happen only when the item collected wants to merge with the ones who has been loadede from the save file. 
                                    //Due to the fact that the itemData is not the same as the one in the dictionary, 
                                    //we need to update it to the one in the dictionary to secure that the ItemData is the same as the one in the dictionary.
                item = entry.Value;
                itemExists = true;
                break;
            }
        }
        // If the item is already in the inventory, increase the quantity
        if (itemExists)
        {
            item.AddQuantity();
            Debug.Log($"{itemData.ItemName} total stack is now {item.Quantity}");
            OnInventoryChange?.Invoke(inventoryItems);
        }
        // If the item is not in the inventory, add it
        else
        {
            InventoryItem newInventoryItem = new InventoryItem(itemData);
            inventoryItems.Add(newInventoryItem);
            inventoryDictionary.Add(itemData, newInventoryItem);
            Debug.Log($"Added {itemData.ItemName} to inventory for the first time.");
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
            Debug.Log($"Removed a unit of {itemData.ItemName}");
            OnInventoryChange?.Invoke(inventoryItems);
        }
    }

    public void LaunchInventoryChange()
    {
        OnInventoryChange?.Invoke(inventoryItems);
    }

    public void LoadData(GameData gameData)
    {
        List<InventoryItemSerializable> inventoryItemSerializables = gameData.inventoryItems;

        inventoryItems = inventoryItemSerializables.Select(itemSerializable =>
        {
            ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
            itemData.ItemName = itemSerializable.ItemDataSerializable.ItemName;
            itemData.ItemDescription = itemSerializable.ItemDataSerializable.ItemDescription;
            itemData.ItemSprite = Resources.Load<Sprite>(itemSerializable.ItemDataSerializable.ItemSpriteName);
            itemData.HealthBoost = itemSerializable.ItemDataSerializable.HealthBoost;
            itemData.AttackBoost = itemSerializable.ItemDataSerializable.AttackBoost;
            itemData.SpiritualEnergyBoost = itemSerializable.ItemDataSerializable.SpiritualEnergyBoost;
            itemData.ConditionID = (ConditionID)itemSerializable.ItemDataSerializable.ConditionID;
            return new InventoryItem(itemData) { Quantity = itemSerializable.Quantity };
        }).ToList();

        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();
        foreach (InventoryItem item in inventoryItems)
        {
            inventoryDictionary.Add(item.ItemData, item);
        }
        LaunchInventoryChange();
    }

    public void SaveData(GameData gameData)
    {
        List<InventoryItemSerializable> inventoryItemsSerializable = inventoryItems.Select(item => new InventoryItemSerializable(item)).ToList();
        gameData.inventoryItems = inventoryItemsSerializable;
    }
}
