using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public Tilemap objectTilemap;
    public Tile newTile;
    public float speed = 2f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Получаем позицию мыши в мировых координатах
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Преобразуем мировые координаты в координаты сетки Tilemap
            Vector3Int tilePosition = objectTilemap.WorldToCell(mouseWorldPos);

            // Устанавливаем новый тайл на Tilemap
            PlaceTile(newTile, tilePosition);
        }
        
        
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

    private void PlaceTile(Tile tile, Vector3Int position)
    {
        objectTilemap.SetTile(position, tile);
    }
}
