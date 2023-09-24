using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public GameObject player; 
    private int distanceThreshold = 1;
    
    private Dictionary<int, Queue<GameObject>> tilePools = new Dictionary<int, Queue<GameObject>>();
    private HashSet<Vector2Int> currentChunks = new HashSet<Vector2Int>();

    Dictionary<int, GameObject> tileset;
    Dictionary<int, GameObject> tile_groups;
    public GameObject prefab_plains;
    public GameObject prefab_forest;
    public GameObject prefab_hills;
    public GameObject prefab_mountains;
    private int chunkSize = 25;

    int map_width = 0;
    int map_height = 0;

    private Dictionary<Vector2Int, int> noise_grid = new Dictionary<Vector2Int, int>();
    private Dictionary<Vector2Int, GameObject> tile_grid = new Dictionary<Vector2Int, GameObject>();


    float magnification = 15.0f;

    int x_offset = 23325;
    int y_offset = 23325;

    void Awake()
    {
        // Инициализация словарей, генерация первоначальной карты
        CreateTileset();
        CreateTileGroups();
        InitializePools();
        GenerateMap();
    }
    void InitializePools()
    {
        foreach (int id in tileset.Keys)
        {
            tilePools[id] = new Queue<GameObject>();
        }
    }

    void CreateTileset()
    {
        tileset = new Dictionary<int, GameObject>();
        tileset.Add(0, prefab_plains);
        tileset.Add(1, prefab_forest);
        tileset.Add(2, prefab_hills);
        tileset.Add(3, prefab_mountains);
    }

    void CreateTileGroups()
    {
        tile_groups = new Dictionary<int, GameObject>();
        foreach (KeyValuePair<int, GameObject> prefab_pair in tileset)
        {
            GameObject tile_group = new GameObject(prefab_pair.Value.name);
            tile_group.transform.parent = gameObject.transform;
            tile_group.transform.localPosition = new Vector3(0, 0, 0);
            tile_groups.Add(prefab_pair.Key, tile_group);
        }
    }

    void GenerateMap()
    {
        for (int x = 0; x < map_width; x++)
        {
            for (int y = 0; y < map_height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                int tile_id = GetIdUsingPerlin(x, y);
                noise_grid[pos] = tile_id;
                GameObject tile = CreateTile(tile_id, x, y);
                tile_grid[pos] = tile;
            }
        }
    }

    int GetIdUsingPerlin(int x, int y)
    {
        float raw_perlin = Mathf.PerlinNoise(
            (x - x_offset) / magnification,
            (y - y_offset) / magnification
        );
        
        float clamp_perlin = Mathf.Clamp01(raw_perlin);
        
        if (clamp_perlin < 0.2f) return 0;
        if (clamp_perlin < 0.3f) return 1;
        if (clamp_perlin < 0.8f) return 2;
    
        return 3;
        
        // float scaled_perlin = clamp_perlin * tileset.Count;
        //
        // if (scaled_perlin == tileset.Count)
        // {
        //     scaled_perlin = (tileset.Count - 1);
        // }
        //
        // return Mathf.FloorToInt(scaled_perlin);
    }

    
    GameObject CreateTile(int tile_id, int x, int y)
    {
        Vector2Int pos = new Vector2Int(x, y);
        GameObject tile = null;

        if (tilePools[tile_id].Count > 0)
        {
            tile = tilePools[tile_id].Dequeue();
            tile.SetActive(true);
        }
        else
        {
            tile = Instantiate(tileset[tile_id], tile_groups[tile_id].transform);
        }

        tile.name = $"tile_x{x}_y{y}";
        tile.transform.position = new Vector3(x, y, 0);
        tile_grid[pos] = tile;

        return tile;
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
                Vector2Int pos = new Vector2Int(x, y);
                if (tile_grid.ContainsKey(pos))
                {
                    Destroy(tile_grid[pos]);
                    tile_grid.Remove(pos);
                }
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
        for (int x = startX; x < startX + chunkWidth; x++)
        {
            for (int y = startY; y < startY + chunkHeight; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                int tile_id = GetIdUsingPerlin(x, y);
            
                noise_grid[pos] = tile_id;
            
                GameObject tile = CreateTile(tile_id, x, y);
            
                tile_grid[pos] = tile;
            }
        }
    }

    
}
