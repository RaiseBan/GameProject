using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class UsageOfItem : MonoBehaviour
{
    private ItemData _item;
    private InventoryVisual inventory;
    private Transform transfInventory;
    private QuickslotInventory quickslot;
    private Player _player;
    private Tilemap tileMap;
    
    private Sprite selectedSprite;
    void Start(){
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>(); 
        tileMap = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<Tilemap>();
        quickslot = GameObject.FindGameObjectWithTag("QuickSlots").GetComponent<QuickslotInventory>();
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryVisual>();
        transfInventory = GameObject.FindGameObjectWithTag("QuickSlots").transform;
        selectedSprite = GameObject.FindGameObjectWithTag("QuickSlots").GetComponent<QuickslotInventory>().selectedSprite;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _item = GameObject.FindGameObjectWithTag("QuickSlots").GetComponent<QuickslotInventory>().currentItem;
            Debug.Log("najal");
            Debug.Log(_item);
            if (_item != null)
            {
                Debug.Log("proerka 1");
                if (!inventory.isOpened /*&& transfInventory.GetChild(currentQuickslotID).GetComponent<Image>().sprite == selectedSprite*/)
                {   
                    Debug.Log("nachalo usage");
                    if (_item.itemType == ItemType.Tool)
                    {

                    }

                    if (_item.itemType == ItemType.Block)
                    {

                        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        Vector3Int cellPosition = tileMap.WorldToCell(mousePosition); // Конвертация в координаты сетки
                        tileMap.SetTile(cellPosition, _item.tile);
                        quickslot.RemoveConsumableItem();
                    }

                    if (_item.itemType == ItemType.Food)
                    {
                        //TODO: сделать скрипты к каждому предмету 
                        if(_player.HP + _item.changeHealth <= 100.0){_player.HP += _item.changeHealth;}
                        else{_player.HP = 100;}
                        Debug.Log(_player.HP+"h");
                        if(_player.Satiety + _item.changeHunger <= 100){_player.Satiety += _item.changeHunger;}
                        else{_player.Satiety = 100;}
                        Debug.Log(_player.Satiety+ "s");
                        quickslot.RemoveConsumableItem();
                    }

                    if (_item.itemType == ItemType.Default)
                    {

                    }

                    if (_item.itemType == ItemType.Weapon)
                    {
                        
                    }
                    
                    
                }
            }
        }
        
    }
    
}
