using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Potion : MonoBehaviour, ICollectible
{
    public static event HandleGemCollected OnPotionCollected;
    public delegate void HandleGemCollected(ItemData itemData);
    public ItemData itemData;

    public void Collect()
    {
        OnPotionCollected?.Invoke(itemData);
        Debug.Log("Potion collected from Potion.cs");
        Destroy(gameObject);
    }
}
