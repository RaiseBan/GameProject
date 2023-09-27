using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;  // <-- Не забудьте про это

public class MapGeneration : MonoBehaviour
{
    
    public GameObject player;
    public Tilemap tilemap; // <-- Добавлен Tilemap
    public Tile[] tiles; // <-- Массив тайлов
    public event Action<Vector2Int, Dictionary<Vector2Int, Tile>, Dictionary<Vector2Int, Tile>> OnChunkGenerated;
    
    private int distanceThreshold = 1;
    public HashSet<Vector2Int> currentChunks = new HashSet<Vector2Int>();

    int chunkSize = 25;

    float magnification = 15.0f;
    int x_offset = 23325;
    int y_offset = 23325;
    

    // void Awake()
    // {
    //     GenerateMap();
    // }
    //
    // void GenerateMap()
    // {
    //     BoundsInt bounds = tilemap.cellBounds;
    //     // TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
    //     
    //     for (int x = 0; x <= bounds.size.x; x++)
    //     {
    //         for (int y = 0; y <= bounds.size.y; y++)
    //         {
    //             Vector3Int tilePosition = new Vector3Int(x, y, 0);
    //             int tile_id = GetTileIdUsingPerlin(x, y);
    //             tilemap.SetTile(tilePosition, tiles[tile_id]); // <-- Задаем тайл
    //         }
    //     }
    // }

    int GetTileIdUsingPerlin(int x, int y)
    {
        float raw_perlin = Mathf.PerlinNoise(
            (x - x_offset) / magnification,
            (y - y_offset) / magnification
        );

        float clamp_perlin = Mathf.Clamp01(raw_perlin);
        
        if (clamp_perlin < 0.2f) return 0;
        if (clamp_perlin < 1f) return 1;
        return 1;
    }
    public void LoadTiles()
    {
        // Здесь код для загрузки тайлов в ваш массив tiles
        // Например, вы можете загрузить их из Resources folder
        tiles = Resources.LoadAll<Tile>("Path/To/Your/Tiles");
    }

    void Update()
    {
        CheckPlayerDistance();
    }
    
    
    void CheckPlayerDistance()
    {
        Vector3 playerPosition = player.transform.position;
        Vector2Int playerChunkPosition = new Vector2Int(
            Mathf.FloorToInt(playerPosition.x / chunkSize),
            Mathf.FloorToInt(playerPosition.y / chunkSize)
        );

        GenerateChunkIfNecessary(playerChunkPosition);
        RemoveDistantChunks(playerChunkPosition);
    }

    void RemoveDistantChunks(Vector2Int playerChunkPosition)
    {
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();
        foreach (Vector2Int chunkPosition in currentChunks)
        {
            if (Mathf.Abs(chunkPosition.x - playerChunkPosition.x) > distanceThreshold ||
                Mathf.Abs(chunkPosition.y - playerChunkPosition.y) > distanceThreshold)
            {
                chunksToRemove.Add(chunkPosition);
            }
        }

        foreach (Vector2Int chunkPosition in chunksToRemove)
        {
            RemoveChunk(chunkPosition);
        }
    }

    void RemoveChunk(Vector2Int chunkPosition)
    {
        int startX = chunkPosition.x * chunkSize;
        int startY = chunkPosition.y * chunkSize;
        for (int x = startX; x < startX + chunkSize; x++)
        {
            for (int y = startY; y < startY + chunkSize; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                tilemap.SetTile(pos, null); // <-- Удаляем тайл
            }
        }

        currentChunks.Remove(chunkPosition);
    }

    void GenerateChunkIfNecessary(Vector2Int playerChunkPosition)
    {
        for (int dx = -distanceThreshold; dx <= distanceThreshold; dx++)
        {
            for (int dy = -distanceThreshold; dy <= distanceThreshold; dy++)
            {
                Vector2Int chunkPosition = new Vector2Int(
                    (playerChunkPosition.x + dx),
                    (playerChunkPosition.y + dy)
                );

                if (!currentChunks.Contains(chunkPosition))
                {
                    currentChunks.Add(chunkPosition);
                    GenerateChunk(chunkPosition.x * chunkSize, chunkPosition.y * chunkSize, chunkSize, chunkSize);
                }
            }
        }
    }

    void GenerateChunk(int startX, int startY, int chunkWidth, int chunkHeight)
    {
        Dictionary<Vector2Int, Tile> tileDictionary = new Dictionary<Vector2Int, Tile>();
        Dictionary<Vector2Int, Tile> borderTiles = new Dictionary<Vector2Int, Tile>();
        for (int x = startX; x < startX + chunkWidth; x++)
        {
            for (int y = startY; y < startY + chunkHeight; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                int tile_id = GetTileIdUsingPerlin(x, y);
                tilemap.SetTile(pos, tiles[tile_id]);
                Vector2Int key = new Vector2Int(x, y);
                tileDictionary.Add(key, tiles[tile_id]);
            }
        }
        int borderSize = 1;  // Размер пограничной зоны
        for (int x = startX - borderSize; x < startX + chunkWidth + borderSize; x++)
        {
            for (int y = startY - borderSize; y < startY + chunkHeight + borderSize; y++)
            {
                // Пропускаем тайлы, которые уже есть в основном чанке
                int tile_id = GetTileIdUsingPerlin(x, y);
                Vector2Int key = new Vector2Int(x, y);
                borderTiles.Add(key, tiles[tile_id]);  // <-- Сохраняем в словаре
            }
        }

        OnChunkGenerated?.Invoke(new Vector2Int(startX, startY), tileDictionary, borderTiles);
    }

 
}