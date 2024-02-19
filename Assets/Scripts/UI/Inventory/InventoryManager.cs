using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    InventorySlot currentSlot;
    public GameObject slotPrefab;
    public InventoryDescription inventoryDescription;
    public Transform inventorySlotsPanel;
    [SerializeField] Button useButton;
    List<InventorySlot> inventorySlots = new List<InventorySlot>(25);
    public static event Action<ItemData> OnItemUsed;

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
    void Update()
    {
        // Disable the use button if there is no item selected
        useButton.enabled = currentSlot != null;
    }

    void ResetInventory()
    {
        foreach (Transform slot in inventorySlotsPanel)
        {
            Destroy(slot.gameObject);
        }
        inventorySlots = new List<InventorySlot>(25);
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
        currentSlot = slot;
    }

    void CreateInventorySlot()
    {
        GameObject slot = Instantiate(slotPrefab);
        slot.transform.SetParent(inventorySlotsPanel, false);
        InventorySlot newInventorySlot = slot.GetComponent<InventorySlot>();
        newInventorySlot.ClearSlot();
        inventorySlots.Add(newInventorySlot);
    }

    public void UseItem()
    {
        if (currentSlot != null)
        {
            currentSlot.Item.ItemData.UseItem();
            OnItemUsed?.Invoke(currentSlot.Item.ItemData);
        }
    }
}
