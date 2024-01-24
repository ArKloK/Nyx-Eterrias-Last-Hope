using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject slotPrefab;
    public InventoryDescription inventoryDescription;
    public Transform inventorySlotsPanel;
    public string descriptionText;
    public List<InventorySlot> inventorySlots = new List<InventorySlot>(24);
    

    void Awake()
    {
        inventoryDescription.ResetDescription();
    }
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
        foreach (Transform slot in inventorySlotsPanel)
        {
            Destroy(slot.gameObject);
        }
        inventorySlots = new List<InventorySlot>(24);
    }

    public void DrawInventory(List<InventoryItem> items)
    {
        Debug.Log("Drawing inventory");
        ResetInventory();
        inventoryDescription.ResetDescription();
        for (int i = 0; i < inventorySlots.Capacity; i++)
        {
            CreateInventorySlot();
        }

        for (int i = 0; i < items.Count; i++)
        {
            inventorySlots[i].DrawSlot(items[i]);
            inventorySlots[i].OnSlotClicked += HandleSlotClicked;
        }
    }

    private void HandleSlotClicked(InventorySlot slot)
    {
        inventoryDescription.SetDescription(slot.itemDescription);
    }

    void CreateInventorySlot()
    {
        GameObject slot = Instantiate(slotPrefab);
        slot.transform.SetParent(inventorySlotsPanel, false);
        InventorySlot newInventorySlot = slot.GetComponent<InventorySlot>();
        newInventorySlot.ClearSlot();
        inventorySlots.Add(newInventorySlot);
    }
}
