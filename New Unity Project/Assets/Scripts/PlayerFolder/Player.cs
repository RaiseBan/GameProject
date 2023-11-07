using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public IUserItem userItem;
    public float HP;
    public float Satiety;
    public QuickslotInventory quickslotInventory;
    // public Tilemap objectTilemap;
    // public Tile newTile;
    public float speed = 2f;
    public InventoryVisual inventory;
    
    private List<Item> placeableItems; // TODO: Сюда надо прикрутить предметы из инветоря, которые можно будет ставить на карту
    
    private void Start()
    {
        HP = 80f;
        Debug.Log("HP "+ HP);
        userItem = new UsageOfItem();
    }
    
    private void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     // Получаем позицию мыши в мировых координатах
        //     Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     // Преобразуем мировые координаты в координаты сетки Tilemap
        //     Vector3Int tilePosition = objectTilemap.WorldToCell(mouseWorldPos);

        //     // Устанавливаем новый тайл на Tilemap
        //     // PlaceTile(tilePosition);
        // }
        
        
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(transform.up * this.speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(transform.right * this.speed * Time.deltaTime * (-1));
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(transform.up * this.speed * Time.deltaTime * (-1));
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(transform.right * this.speed * Time.deltaTime);
        }
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Item>())
        {
            if (collision.gameObject.GetComponent<Item>().item != null)
            {
                inventory.AddItem(collision.gameObject.GetComponent<Item>().item, collision.gameObject.GetComponent<Item>().amount);
                if (inventory.canTake == true)
                {
                    Destroy(collision.collider.gameObject);
                }
                else
                {
                    collision.gameObject.GetComponent<Item>().amount = inventory.cantTake;
                }

            }
        }

    }

    // private Item getCurrentItem()
    // {
    //     return new (); // TODO: здесь надо возвращать выбранный предмет сейчас (я не знаю, у тебя это как тайл или как префаб)
    // }
    
    // private void PlaceTile(Vector3Int position)
    // {
    //     // вызываем метод для получения текущего предмета в инветоре
    //     // делаем проверку можно ли поставить этот предмет на карту. P.S. Это можно сделать с помощью классов. То есть будет 2 класса: Placeable, NotPlaceable
    //     // соответственно делаем проверку классов. И понимаем можно ли поставить этот предмет или нет.
    //     ItemData itemData = quickslotInventory.getQuickItem();
    //     userItem.Usage();
    //     if (itemData.placeable)
    //     {
    //         // objectTilemap.SetTile(position, ); // Ставим на Tilemap новый тайл/префаб (tile - то, что ставим; postition - куда ставим) ВСЕ ЛОГИЧНО)
    //     }
        
        
    // }
}
