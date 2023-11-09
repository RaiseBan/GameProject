using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu(fileName = "Block Item", menuName = "Inventory/Items/New Block item")]
public class BlockItem : ItemData
{
    public float Health = 100f;
    private void Start()
    {
        itemType = ItemType.Block;
        
    }
}

