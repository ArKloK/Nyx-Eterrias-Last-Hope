using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject slotPrefab;
    public List<InventorySlot> inventorySlots = new List<InventorySlot>(32);

    void OnEnable()
    {
        Inventory.OnInventoryChange += DrawInventory;
    }

    void OnDisable()
    {
        Inventory.OnInventoryChange -= DrawInventory;
    }

    void ResetInventory()
    {
        foreach (Transform slot in transform)
        {
            Destroy(slot.gameObject);
        }
        inventorySlots = new List<InventorySlot>(32);
    }

    public void DrawInventory(List<InventoryItem> items)
    {
        Debug.Log("Drawing inventory");
        ResetInventory();
        Debug.Log($"Inventory has {inventorySlots.Capacity} slots");
        for (int i = 0; i < inventorySlots.Capacity; i++)
        {
            CreateInventorySlot();
        }

        for (int i = 0; i < items.Count; i++)
        {
            inventorySlots[i].DrawSlot(items[i]);
        }
    }

    void CreateInventorySlot()
    {
        GameObject slot = Instantiate(slotPrefab);
        slot.transform.SetParent(transform, false);
        InventorySlot newInventorySlot = slot.GetComponent<InventorySlot>();
        newInventorySlot.ClearSlot();
        inventorySlots.Add(newInventorySlot);
    }
}
