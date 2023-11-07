using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class UsageOfItem : MonoBehaviour, IUserItem
{
    public Player player;
    public Tilemap tileMap;
    public void Start(){
        
        tileMap = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<Tilemap>();
    }
    public void Usage<T>(T _item) where T : ItemData
    {
        if (_item.itemType == ItemType.Tool)
        {
            
        }

        if (_item.itemType == ItemType.Block && _item is ItemDataTiles itemDataTiles)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = tileMap.WorldToCell(mousePosition); // Конвертация в координаты сетки
            tileMap.SetTile(cellPosition, itemDataTiles.tile);
        }

        if (_item.itemType == ItemType.Food)
        {
            Debug.Log("Player: " + player + " "); // player.HP - нет почему-то посмотри где задается
            Debug.Log("Item: " + _item + " " + _item.name);
            if(player.HP + _item.changeHealth <= 100.0)
            {
                player.HP =+ _item.changeHealth;
            }
            // Иначе, просто ставим здоровье на 100
            else
            {
                player.HP = 100;
            }
            
            if(player.Satiety + _item.changeHunger <= 100)
            {
                player.Satiety =+ _item.changeHunger;
            }
            // Иначе, просто ставим здоровье на 100
            else
            {
                player.Satiety = 100;
            }
            Debug.Log("hp:" + player.HP);
            Debug.Log("hunger:" + player.Satiety);
        }
        
        if (_item.itemType == ItemType.Default)
        {
            
        }

        if (_item.itemType == ItemType.Weapon)
        {
            
        }
    }
    
}
