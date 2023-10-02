using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 2f;
    public InventoryVisual inventory;
    private void Update()
    {
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
        if(collision.gameObject.GetComponent<Item>())
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
}
