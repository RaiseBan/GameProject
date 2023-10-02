using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Food Item", menuName = "Inventory/Items/New Food Item")]
public class FoodItem : ItemData
{
    public float healByItem;

    private void Start()
    {
        itemType = ItemType.Food;
    }
}
