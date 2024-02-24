using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    InventoryItem item;
    public Image icon;
    public TextMeshProUGUI labelText;
    public TextMeshProUGUI stackSizeText;
    public string itemDescription;

    public InventoryItem Item { get => item; set => item = value; }

    public event Action<InventorySlot> OnSlotClicked;

    public void ClearSlot()
    {
        icon.enabled = false;
        labelText.enabled = false;
        stackSizeText.enabled = false;
    }

    public void DrawSlot (InventoryItem item)
    {
        if (item == null)
        {
            ClearSlot();
            return;
        }

        this.item = item;

        icon.enabled = true;
        labelText.enabled = true;
        stackSizeText.enabled = true;

        icon.sprite = item.ItemData.ItemSprite;
        labelText.text = item.ItemData.ItemName;
        stackSizeText.text = item.Quantity.ToString();
        itemDescription = item.ItemData.ItemDescription;
    }

    public void onPointerClick()
    {
        OnSlotClicked?.Invoke(this);
    }
}
