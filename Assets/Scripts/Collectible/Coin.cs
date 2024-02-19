using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Coin : MonoBehaviour, ICollectible
{
    public static event HandleGemCollected OnCoinCollected;
    public delegate void HandleGemCollected(ItemData itemData);
    public ItemData itemData;

    public void Collect()
    {
        OnCoinCollected?.Invoke(itemData);
        Destroy(gameObject);
    }
}
