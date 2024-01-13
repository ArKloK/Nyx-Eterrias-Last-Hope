using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI labelText;
    public TextMeshProUGUI stackSizeText;
    public string itemDescription;

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

        icon.enabled = true;
        labelText.enabled = true;
        stackSizeText.enabled = true;

        icon.sprite = item.itemData.itemSprite;
        labelText.text = item.itemData.itemName;
        stackSizeText.text = item.quantity.ToString();
        itemDescription = item.itemData.itemDescription;
    }

    public void onPointerClick()
    {
        OnSlotClicked?.Invoke(this);
    }
}
