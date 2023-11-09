using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    
    public float HP;
    public float Satiety;
    public float speed = 2f;
    public InventoryVisual inventory;
    private Rigidbody2D rb;
    
    private List<Item> placeableItems; // TODO: Сюда надо прикрутить предметы из инветоря, которые можно будет ставить на карту


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on the player!");
        }
    }

    private void Start()
    {
        
        Debug.Log(rb);
        HP = 80f;
        Debug.Log("HP "+ HP);
        
    }
    
    private void FixedUpdate()
    {
        Vector2 movement = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            movement += Vector2.up;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement += Vector2.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement += Vector2.down;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement += Vector2.right;
        }

        movement.Normalize(); // Это предотвратит более быстрое движение по диагонали.

        if (movement != Vector2.zero)
        {
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
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
