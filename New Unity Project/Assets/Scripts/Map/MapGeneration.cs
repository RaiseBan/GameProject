using System;
using System.Collections.Generic;
using Map;
using UnityEngine;
using UnityEngine.Tilemaps;  // <-- Не забудьте про это

public class MapGeneration : MonoBehaviour
{
    public GameObject player;
    public NatureTilemapManager natureTilemapManager;
    public Tilemap tilemap; // <-- Добавлен Tilemap
    public Tile[] tiles; // <-- Массив тайлов
    public event Action<Vector2Int, Dictionary<Vector2Int, Tile>, Dictionary<Vector2Int, Tile>> OnChunkGenerated;
    public event Action<Vector2Int, Dictionary<Vector2Int, Tile>, Dictionary<Vector2Int, Tile>> OnGrassNeeded;

    private int distanceThreshold = 1;
    public HashSet<Vector2Int> currentChunks = new HashSet<Vector2Int>();

    int chunkSize = 25;

    float magnification = 15.0f;
    int x_offset = 23325;
    int y_offset = 23325;

    int GetTileIdUsingPerlin(int x, int y) {
        float raw_perlin = Mathf.PerlinNoise(
            (x - x_offset) / magnification,
            (y - y_offset) / magnification
        );

        float clamp_perlin = Mathf.Clamp01(raw_perlin);
    
        if (clamp_perlin < 0.2f) return 0;  // Вода
    
        float grassNoise = GenerateGrassNoise(x, y);

        // Если шум травы показывает, что здесь должна быть трава, и значение clamp_perlin далеко от границы воды...
        if (grassNoise > 0.5f && clamp_perlin > 0.3f) {  // Здесь 0.3 - это допустимое значение, далекое от границы воды (0.2). Вы можете экспериментировать с этим значением для получения желаемого результата.
            return 2;  // ID для травы
        }

        return 1;  // Земля
    }
    float GenerateGrassNoise(int x, int y) {
        // Используем другие параметры для масштабирования, чтобы разнообразить шум
        float grassNoise = Mathf.PerlinNoise(
            (x - x_offset) / (magnification * 0.5f),
            (y - y_offset) / (magnification * 0.5f)
        );
        return grassNoise;
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
                natureTilemapManager.RemoveNatureTrees(chunkPosition);
                natureTilemapManager.RemoveNatureStones(chunkPosition);
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
                    natureTilemapManager.GenerateNatureObjects(chunkPosition, chunkSize);
                    
                    if (natureTilemapManager.getSavedNatureTrees().ContainsKey(chunkPosition) || natureTilemapManager.getSavedNatureRocks().ContainsKey(chunkPosition))
                    {
                        natureTilemapManager.RestoreNatureTrees(chunkPosition);
                        natureTilemapManager.RestoreNatureStones(chunkPosition);
                    }
                    else
                    {
                        natureTilemapManager.GenerateNatureObjects(chunkPosition, chunkSize);
                    }
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
        OnGrassNeeded?.Invoke(new Vector2Int(startX, startY), tileDictionary, borderTiles);
    }

 
}