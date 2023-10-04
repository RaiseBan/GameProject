using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType{Default,Food,Weapon,Tool}

public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public GameObject itemPrefab;
    public Sprite icon;
    public int maxAmount;
    public string itemDescription;
    public bool isConsumeable;

    [Header("Consumable Characteristics")]
    public float changeHealth;
    public float changeHunger;
    public float changeThirst;
}
