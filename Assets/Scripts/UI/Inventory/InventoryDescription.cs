using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDescription : MonoBehaviour
{
    public TextMeshProUGUI itemDescription;

    public void ResetDescription()
    {
        itemDescription.text = "";
    }

    public void SetDescription(string description)
    {
        itemDescription.text = description;
    }
}
