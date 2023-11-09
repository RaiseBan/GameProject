using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum ItemType{Default,Food,Weapon,Tool, Block}

public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public GameObject itemPrefab;
    public Sprite icon;
    public int maxAmount;
    public string itemDescription;
    //public bool isConsumeable;
    
    

    [Header("Consumable Characteristics")]
    public float changeHealth;
    public float changeHunger;

    [Header("Tile")]
    public Tile tile;
    // public bool isPlaceable()
    // {
    //     return placeable;
    // }
}
