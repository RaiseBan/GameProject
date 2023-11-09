using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PostProcessMap : MonoBehaviour
{
    public Tilemap tileMap;
    private MapGeneration mapGeneration; // Переменная, хранящая ссылку на MapGeneration
    private int chunkSize = 25; // должно совпадать с размером чанка в MapGeneration

    private ConcurrentQueue<(Vector2Int, Dictionary<Vector2Int, Tile>, Dictionary<Vector2Int, Tile>)> chunkQueue
        = new ConcurrentQueue<(Vector2Int, Dictionary<Vector2Int, Tile>, Dictionary<Vector2Int, Tile>)>();

    private Dictionary<Vector2Int, Tile> tileDictionary;
    private Dictionary<Vector2Int, Tile> borderTiles;

    public List<Tile> postWaterTiles;
    public List<Tile> postGrassTiles;
    

    void Awake()
    {
        mapGeneration = FindObjectOfType<MapGeneration>();
        // StartPostProcess();
        mapGeneration.OnChunkGenerated += EnqueueChunkForPostProcessing;

        StartCoroutine(ProcessChunkQueue());
    }


    private void EnqueueChunkForPostProcessing(Vector2Int start, Dictionary<Vector2Int, Tile> tileDictionary,
        Dictionary<Vector2Int, Tile> borderTiles)
    {
        chunkQueue.Enqueue((start, new Dictionary<Vector2Int, Tile>(tileDictionary),
            new Dictionary<Vector2Int, Tile>(borderTiles)));
    }

    private IEnumerator ProcessChunkQueue()
    {
        while (true)
        {
            if (chunkQueue.TryDequeue(out var chunkData))
            {
                PostProcessChunk(chunkData.Item1, chunkData.Item2, chunkData.Item3);
            }

            yield return null;
        }
    }

    public void PostProcessChunk(Vector2Int chunk, Dictionary<Vector2Int, Tile> localTileDict,
        Dictionary<Vector2Int, Tile> localBorderTiles)
    {
        int startX = chunk.x - 1; // Расширим на 1 тайл влево
        int startY = chunk.y - 1; // Расширим на 1 тайл вверх
        int endX = chunk.x + chunkSize + 1; // Расширим на 1 тайл вправо
        int endY = chunk.y + chunkSize + 1; // Расширим на 1 тайл вниз

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                List<Tile> tiles = new List<Tile>();

                tiles = GetTileAndBorderTiles(x, y, localBorderTiles);
                TransformTile(tiles, x, y);
            }
        }
    }

    public List<Tile> GetTileAndBorderTiles(int x, int y, Dictionary<Vector2Int, Tile> localTileDict)
    {
        Tile foundTile;
        List<Tile> mainAndBorderTiles = new List<Tile>();

        //l t r b c
        if (localTileDict.TryGetValue(new Vector2Int(x - 1, y + 1), out foundTile))
        {
            mainAndBorderTiles.Add(foundTile);
        }

        if (localTileDict.TryGetValue(new Vector2Int(x, y + 1), out foundTile))
        {
            mainAndBorderTiles.Add(foundTile);
        }

        if (localTileDict.TryGetValue(new Vector2Int(x + 1, y + 1), out foundTile))
        {
            mainAndBorderTiles.Add(foundTile);
        }

        if (localTileDict.TryGetValue(new Vector2Int(x - 1, y), out foundTile))
        {
            mainAndBorderTiles.Add(foundTile);
        }

        if (localTileDict.TryGetValue(new Vector2Int(x, y), out foundTile))
        {
            mainAndBorderTiles.Add(foundTile);
        }

        if (localTileDict.TryGetValue(new Vector2Int(x + 1, y), out foundTile))
        {
            mainAndBorderTiles.Add(foundTile);
        }

        if (localTileDict.TryGetValue(new Vector2Int(x - 1, y - 1), out foundTile))
        {
            mainAndBorderTiles.Add(foundTile);
        }

        if (localTileDict.TryGetValue(new Vector2Int(x, y - 1), out foundTile))
        {
            mainAndBorderTiles.Add(foundTile);
        }

        if (localTileDict.TryGetValue(new Vector2Int(x + 1, y - 1), out foundTile))
        {
            mainAndBorderTiles.Add(foundTile);
        }


        return mainAndBorderTiles;
    }


    public void TransformTile(List<Tile> tiles, int x, int y)
    {
        String mainTileName;
        String otherTileName;
        if (tiles.Count == 9 && tiles[4].name == "water_light")
        {
            mainTileName = "water_light"; 
            otherTileName = "grass_light";
            Process(tiles, x, y, mainTileName, otherTileName, postWaterTiles);
        }
        if (tiles.Count == 9 && tiles[4].name == "grass_dark")
        {
            mainTileName = "grass_dark";
            otherTileName = "grass_light";
            Process(tiles, x, y, mainTileName, otherTileName, postGrassTiles);
        }
    }

    public void Process(List<Tile> tiles, int x, int y, String mainTileName, String otherTileName, List<Tile> tileList)
    {
        if (tiles[5].name == otherTileName && tiles[1].name == otherTileName && tiles[6].name == mainTileName &&
            tiles[3].name == mainTileName && tiles[7].name == mainTileName)
        {
            //top-right angle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[7]);
        }

        if (tiles[3].name == otherTileName && tiles[1].name == otherTileName && tiles[5].name == mainTileName &&
            tiles[8].name == mainTileName && tiles[7].name == mainTileName)
        {
            // top-left angle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[5]);
        }

        if (tiles[7].name == otherTileName && tiles[3].name == otherTileName && tiles[1].name == mainTileName &&
            tiles[2].name == mainTileName && tiles[5].name == mainTileName)
        {
            // bottom-left angle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[21]);
        }

        if (tiles[7].name == otherTileName && tiles[5].name == otherTileName && tiles[0].name == mainTileName &&
            tiles[3].name == mainTileName && tiles[1].name == mainTileName)
        {
            // bottom-right angle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[23]);
        }

        // 0 1 2 
        // 3 4 5 
        // 6 7 8 

        // check sides

        if (tiles[1].name == otherTileName && tiles[3].name == mainTileName && tiles[6].name == mainTileName &&
            tiles[5].name == mainTileName && tiles[7].name == mainTileName && tiles[8].name == mainTileName)
        {
            // top
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[6]);
        }

        if (tiles[7].name == otherTileName && tiles[3].name == mainTileName && tiles[5].name == mainTileName &&
            tiles[0].name == mainTileName && tiles[1].name == mainTileName && tiles[2].name == mainTileName)
        {
            // bottom
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[22]);
        }

        if (tiles[5].name == otherTileName && tiles[1].name == mainTileName && tiles[7].name == mainTileName &&
            tiles[0].name == mainTileName && tiles[3].name == mainTileName && tiles[6].name == mainTileName)
        {
            // right
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[15]);
        }

        if (tiles[3].name == otherTileName && tiles[1].name == mainTileName && tiles[7].name == mainTileName &&
            tiles[2].name == mainTileName && tiles[5].name == mainTileName && tiles[8].name == mainTileName)
        {
            // left
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[13]);
        }

        // 0 1 2 
        // 3 4 5 
        // 6 7 8 

        // inner angles

        if (tiles[1].name == mainTileName && tiles[5].name == mainTileName && tiles[2].name == otherTileName &&
            tiles[0].name == mainTileName && tiles[3].name == mainTileName && tiles[6].name == mainTileName &&
            tiles[7].name == mainTileName && tiles[8].name == mainTileName)
        {
            // top-right (inner)
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[36]);
        }

        if (tiles[1].name == mainTileName && tiles[3].name == mainTileName && tiles[0].name == otherTileName &&
            tiles[2].name == mainTileName && tiles[5].name == mainTileName && tiles[6].name == mainTileName &&
            tiles[7].name == mainTileName && tiles[8].name == mainTileName)
        {
            // top-left (inner)
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[37]);
        }

        if (tiles[3].name == mainTileName && tiles[7].name == mainTileName && tiles[6].name == otherTileName &&
            tiles[0].name == mainTileName && tiles[2].name == mainTileName && tiles[1].name == mainTileName &&
            tiles[5].name == mainTileName && tiles[8].name == mainTileName)
        {
            // bottom-left (inner)
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[29]);
        }

        if (tiles[5].name == mainTileName && tiles[7].name == mainTileName && tiles[8].name == otherTileName &&
            tiles[0].name == mainTileName && tiles[1].name == mainTileName && tiles[2].name == mainTileName &&
            tiles[3].name == mainTileName && tiles[6].name == mainTileName)
        {
            // bottom-right (inner)
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[28]);
        }


        // 0 1 2 
        // 3 4 5 
        // 6 7 8 

        // dot 
        if (tiles[1].name == otherTileName && tiles[5].name == otherTileName && tiles[7].name == otherTileName &&
            tiles[3].name == otherTileName)
        {
            // dot
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[0]);
        }

        // 0 1 2 
        // 3 4 5 
        // 6 7 8 

        // dot with 1/2 water
        if (tiles[1].name == otherTileName && tiles[3].name == otherTileName && tiles[7].name == otherTileName &&
            tiles[5].name == mainTileName)
        {
            // left-upper-bottom
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[1]);
        }


        if (tiles[1].name == otherTileName && tiles[3].name == otherTileName && tiles[5].name == otherTileName &&
            tiles[7].name == mainTileName)
        {
            // right-upper-left
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[4]);
        }


        if (tiles[1].name == otherTileName && tiles[5].name == otherTileName && tiles[7].name == otherTileName &&
            tiles[3].name == mainTileName)
        {
            // right-upper-bottom
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[3]);
        }


        if (tiles[3].name == otherTileName && tiles[5].name == otherTileName && tiles[7].name == otherTileName &&
            tiles[1].name == mainTileName)
        {
            // right-left-bottom
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[20]);
        }


        if (tiles[1].name == otherTileName && tiles[7].name == otherTileName && tiles[3].name == mainTileName &&
            tiles[5].name == mainTileName)
        {
            // upper-bottom
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[2]);
        }


        if (tiles[5].name == otherTileName && tiles[3].name == otherTileName && tiles[7].name == mainTileName &&
            tiles[1].name == mainTileName)
        {
            // left-right
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[12]);
        }


        // 0 1 2 
        // 3 4 5 
        // 6 7 8

        // closed angles
        if (tiles[3].name == otherTileName && tiles[1].name == otherTileName && tiles[8].name == otherTileName &&
            tiles[5].name == mainTileName && tiles[7].name == mainTileName)
        {
            // bottom-right 
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[8]);
        }


        if (tiles[1].name == otherTileName && tiles[5].name == otherTileName && tiles[6].name == otherTileName &&
            tiles[3].name == mainTileName && tiles[7].name == mainTileName)
        {
            // bottom-left 
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[9]);
        }


        if (tiles[3].name == otherTileName && tiles[7].name == otherTileName && tiles[2].name == otherTileName &&
            tiles[1].name == mainTileName && tiles[5].name == mainTileName)
        {
            // top-right
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[16]);
        }


        if (tiles[5].name == otherTileName && tiles[7].name == otherTileName && tiles[0].name == otherTileName &&
            tiles[1].name == mainTileName && tiles[3].name == mainTileName)
        {
            // top-left
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[17]);
        }

        // 0 1 2 
        // 3 4 5 
        // 6 7 8


        // T-tiles
        if (tiles[3].name == otherTileName && tiles[0].name == otherTileName && tiles[6].name == otherTileName &&
            tiles[2].name == otherTileName && tiles[8].name == otherTileName && tiles[1].name == mainTileName &&
            tiles[7].name == mainTileName && tiles[5].name == mainTileName)
        {
            // top-right-bottom 
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[10]);
        }


        if (tiles[1].name == otherTileName && tiles[0].name == otherTileName && tiles[2].name == otherTileName &&
            tiles[6].name == otherTileName && tiles[8].name == otherTileName && tiles[3].name == mainTileName &&
            tiles[5].name == mainTileName && tiles[7].name == mainTileName)
        {
            // bottom-right-left
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[11]);
        }


        if (tiles[6].name == otherTileName && tiles[7].name == otherTileName && tiles[8].name == otherTileName &&
            tiles[0].name == otherTileName && tiles[2].name == otherTileName && tiles[3].name == mainTileName &&
            tiles[5].name == mainTileName && tiles[1].name == mainTileName)
        {
            // top-right-left
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[18]);
        }


        if (tiles[2].name == otherTileName && tiles[5].name == otherTileName && tiles[8].name == otherTileName &&
            tiles[0].name == otherTileName && tiles[6].name == otherTileName && tiles[3].name == mainTileName &&
            tiles[1].name == mainTileName && tiles[7].name == mainTileName)
        {
            // top-left-bottom 
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[19]);
        }

        // 0 1 2 
        // 3 4 5 
        // 6 7 8

        //1 wall 1 corner
        if (tiles[3].name == otherTileName && tiles[8].name == otherTileName && tiles[1].name == mainTileName &&
            tiles[5].name == mainTileName && tiles[2].name == mainTileName && tiles[7].name == mainTileName)
        {
            // top-right-bottom-rightTopAngle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[24]);
        }


        if (tiles[5].name == otherTileName && tiles[6].name == otherTileName && tiles[0].name == mainTileName &&
            tiles[1].name == mainTileName && tiles[3].name == mainTileName && tiles[7].name == mainTileName)
        {
            // top-right-bottom-rightTopAngle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[25]);
        }


        if (tiles[1].name == otherTileName && tiles[8].name == otherTileName && tiles[3].name == mainTileName &&
            tiles[6].name == mainTileName && tiles[7].name == mainTileName && tiles[5].name == mainTileName)
        {
            // top-right-bottom-rightTopAngle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[26]);
        }


        if (tiles[1].name == otherTileName && tiles[6].name == otherTileName && tiles[3].name == mainTileName &&
            tiles[5].name == mainTileName && tiles[7].name == mainTileName && tiles[8].name == mainTileName)
        {
            // top-right-bottom-rightTopAngle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[27]);
        }


        if (tiles[3].name == otherTileName && tiles[2].name == otherTileName && tiles[1].name == mainTileName &&
            tiles[5].name == mainTileName && tiles[7].name == mainTileName && tiles[8].name == mainTileName)
        {
            // top-right-bottom-rightTopAngle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[32]);
        }


        if (tiles[5].name == otherTileName && tiles[0].name == otherTileName && tiles[1].name == mainTileName &&
            tiles[3].name == mainTileName && tiles[7].name == mainTileName && tiles[6].name == mainTileName)
        {
            // top-right-bottom-rightTopAngle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[33]);
        }


        if (tiles[2].name == otherTileName && tiles[7].name == otherTileName && tiles[1].name == mainTileName &&
            tiles[3].name == mainTileName && tiles[0].name == mainTileName && tiles[5].name == mainTileName)
        {
            // top-right-bottom-rightTopAngle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[34]);
        }


        if (tiles[6].name == otherTileName && tiles[0].name == otherTileName && tiles[1].name == mainTileName &&
            tiles[3].name == mainTileName && tiles[2].name == mainTileName && tiles[5].name == mainTileName)
        {
            // top-right-bottom-rightTopAngle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[35]);
        }

        // 0 1 2 
        // 3 4 5 
        // 6 7 8

        //3 inner angles

        if (tiles[2].name == otherTileName && tiles[6].name == otherTileName && tiles[8].name == otherTileName &&
            tiles[1].name == mainTileName && tiles[0].name == mainTileName && tiles[3].name == mainTileName &&
            tiles[7].name == mainTileName && tiles[5].name == mainTileName)
        {
            // top-right-bottom-rightTopAngle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[30]);
        }


        if (tiles[0].name == otherTileName && tiles[6].name == otherTileName && tiles[8].name == otherTileName &&
            tiles[1].name == mainTileName && tiles[2].name == mainTileName && tiles[5].name == mainTileName &&
            tiles[7].name == mainTileName && tiles[3].name == mainTileName)
        {
            // top-right-bottom-rightTopAngle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[31]);
        }


        if (tiles[0].name == otherTileName && tiles[2].name == otherTileName && tiles[8].name == otherTileName &&
            tiles[1].name == mainTileName && tiles[5].name == mainTileName && tiles[3].name == mainTileName &&
            tiles[7].name == mainTileName && tiles[6].name == mainTileName)
        {
            // top-right-bottom-rightTopAngle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[38]);
        }


        if (tiles[0].name == otherTileName && tiles[2].name == otherTileName && tiles[6].name == otherTileName &&
            tiles[1].name == mainTileName && tiles[5].name == mainTileName && tiles[3].name == mainTileName &&
            tiles[7].name == mainTileName && tiles[8].name == mainTileName)
        {
            // top-right-bottom-rightTopAngle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[39]);
        }

        // 0 1 2 
        // 3 4 5 
        // 6 7 8

        //2 inner angles

        if (tiles[0].name == otherTileName && tiles[2].name == otherTileName && tiles[1].name == mainTileName &&
            tiles[3].name == mainTileName && tiles[5].name == mainTileName && tiles[6].name == mainTileName &&
            tiles[7].name == mainTileName && tiles[8].name == mainTileName)
        {
            // top-right-bottom-rightTopAngle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[40]);
        }


        if (tiles[6].name == otherTileName && tiles[8].name == otherTileName && tiles[0].name == mainTileName &&
            tiles[3].name == mainTileName && tiles[1].name == mainTileName && tiles[2].name == mainTileName &&
            tiles[5].name == mainTileName && tiles[7].name == mainTileName)
        {
            // top-right-bottom-rightTopAngle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[41]);
        }


        if (tiles[2].name == otherTileName && tiles[8].name == otherTileName && tiles[5].name == mainTileName &&
            tiles[3].name == mainTileName && tiles[0].name == mainTileName && tiles[6].name == mainTileName &&
            tiles[7].name == mainTileName && tiles[1].name == mainTileName)
        {
            // top-right-bottom-rightTopAngle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[42]);
        }


        if (tiles[0].name == otherTileName && tiles[6].name == otherTileName && tiles[1].name == mainTileName &&
            tiles[2].name == mainTileName && tiles[5].name == mainTileName && tiles[3].name == mainTileName &&
            tiles[7].name == mainTileName && tiles[8].name == mainTileName)
        {
            // top-right-bottom-rightTopAngle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[43]);
        }


        if (tiles[6].name == otherTileName && tiles[2].name == otherTileName && tiles[1].name == mainTileName &&
            tiles[3].name == mainTileName && tiles[0].name == mainTileName && tiles[5].name == mainTileName &&
            tiles[7].name == mainTileName && tiles[8].name == mainTileName)
        {
            // top-right-bottom-rightTopAngle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[44]);
        }


        if (tiles[0].name == otherTileName && tiles[8].name == otherTileName && tiles[1].name == mainTileName &&
            tiles[2].name == mainTileName && tiles[5].name == mainTileName && tiles[6].name == mainTileName &&
            tiles[7].name == mainTileName && tiles[3].name == mainTileName)
        {
            // top-right-bottom-rightTopAngle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[45]);
        }

        //4 inner angles

        if (tiles[0].name == otherTileName && tiles[2].name == otherTileName && tiles[6].name == otherTileName &&
            tiles[8].name == otherTileName && tiles[1].name == mainTileName && tiles[3].name == mainTileName &&
            tiles[5].name == mainTileName && tiles[7].name == mainTileName)
        {
            // top-right-bottom-rightTopAngle
            tileMap.SetTile(new Vector3Int(x, y, 0), tileList[45]);
        }
    }


    // public void StartPostProcess()
    // {
    //     Parallel.ForEach(mapGeneration.currentChunks, chunk =>
    //     {
    //         PostProcessChunk(chunk);
    //     });
    // }
}