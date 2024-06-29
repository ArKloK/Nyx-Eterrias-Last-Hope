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
    public static event Action<ItemData> OnTBItemUsedUpdateHP;

    void Awake()
    {
        inventoryDescription.ResetDescription();
    }
    void OnEnable()
    {
        Inventory.OnInventoryChange += DrawInventory;
        BattleSystem.OnHumanModelHeals += HumanModelUseItem;
    }

    void OnDisable()
    {
        Inventory.OnInventoryChange -= DrawInventory;
        BattleSystem.OnHumanModelHeals -= HumanModelUseItem;
    }
    void Update()
    {
        // Disable the use button if there is no item selected
        useButton.gameObject.SetActive(currentSlot != null);
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
        Debug.Log("Slot clicked");
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
            ConsumeItem();
            OnItemUsed?.Invoke(currentSlot.Item.ItemData);
            OnTBItemUsedUpdateHP?.Invoke(currentSlot.Item.ItemData);
        }
    }
    public void ConsumeItem()
    {
        int currentHealthPoints;
        currentHealthPoints = PlayerStats.CurrentHealthPoints += currentSlot.Item.ItemData.HealthBoost;
        Debug.Log("Using " + currentSlot.Item.ItemData.ItemName);

        // Clamp the health and spiritual energy points to their maximum values in case a max boost potion is used
        Debug.Log("Player stats Max health points: " + PlayerStats.MaxHealthPoints);
        PlayerStats.CurrentHealthPoints = Mathf.Clamp(currentHealthPoints, 0, PlayerStats.MaxHealthPoints);
        //--------------------------------

        Debug.Log("Player Stats 2 current health points: " + PlayerStats.CurrentHealthPoints);

        PlayerStats.AttackPower += currentSlot.Item.ItemData.AttackBoost;
        FindFirstObjectByType <PlayerController>().SetLocalStats();
        TBCharacterUnit[] tBCharacterUnit = FindObjectsByType<TBCharacterUnit>(FindObjectsSortMode.None);
        foreach (TBCharacterUnit tBCharacter in tBCharacterUnit)
        {
            if (tBCharacter != null && !tBCharacter.Character.CharacterData.IsEnemy)
            {
                Debug.Log("Setting stats");
                tBCharacter.Character.setStats();
            }
        }
    }
    public void HumanModelUseItem(InventoryItem inventoryItem)
    {
        int currentHealthPoints;
        currentHealthPoints = PlayerStats.CurrentHealthPoints += inventoryItem.ItemData.HealthBoost;
        PlayerStats.CurrentHealthPoints = Mathf.Clamp(currentHealthPoints, 0, PlayerStats.MaxHealthPoints);

        FindFirstObjectByType <PlayerController>().SetLocalStats();
        TBCharacterUnit[] tBCharacterUnit = FindObjectsByType<TBCharacterUnit>(FindObjectsSortMode.None);
        foreach (TBCharacterUnit tBCharacter in tBCharacterUnit)
        {
            if (tBCharacter != null && !tBCharacter.Character.CharacterData.IsEnemy)
            {
                Debug.Log("Setting stats Human Model Method");
                tBCharacter.Character.setStats();
            }
        }
        OnItemUsed?.Invoke(inventoryItem.ItemData);
        OnTBItemUsedUpdateHP?.Invoke(inventoryItem.ItemData);
    }
}
