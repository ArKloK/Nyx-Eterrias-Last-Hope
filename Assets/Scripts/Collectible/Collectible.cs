using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Collectible : MonoBehaviour, ICollectible, IDataPersistence
{
    [SerializeField] private string id;
    public static event HandleCollectibleCollected OnCollectibleCollected;
    public delegate void HandleCollectibleCollected(ItemData itemData);
    [SerializeField] ItemData itemData;
    private bool isCollected;

    public string Id { get => id; set => id = value; }
    public bool IsCollected { get => isCollected; set => isCollected = value; }

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = itemData.ItemSprite;
        //This assigns a new GUID to the collectible if it doesn't have one
        if (string.IsNullOrEmpty(id))
        {
            id = Guid.NewGuid().ToString();
        }
    }

    public void Collect()
    {
        if (isCollected) return;
        isCollected = true;
        OnCollectibleCollected?.Invoke(itemData);
        Destroy(gameObject);
    }

    public void LoadData(GameData gameData)
    {
        gameData.collectibles.TryGetValue(id, out isCollected);
        if (isCollected)
        {
            OnCollectibleCollected?.Invoke(itemData);
            Destroy(gameObject);
        }
    }

    public void SaveData(GameData gameData)
    {
        if (gameData.collectibles.ContainsKey(id))
        {
            gameData.collectibles.Remove(id);
        }
        gameData.collectibles.Add(id, isCollected);
    }
}
