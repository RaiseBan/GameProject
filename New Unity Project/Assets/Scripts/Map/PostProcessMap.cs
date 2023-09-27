
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
    private MapGeneration mapGeneration;  // Переменная, хранящая ссылку на MapGeneration
    private int chunkSize = 25;  // должно совпадать с размером чанка в MapGeneration
    private ConcurrentQueue<(Vector2Int, Dictionary<Vector2Int, Tile>, Dictionary<Vector2Int, Tile>)> chunkQueue 
        = new ConcurrentQueue<(Vector2Int, Dictionary<Vector2Int, Tile>, Dictionary<Vector2Int, Tile>)>();
    private Dictionary<Vector2Int, Tile> tileDictionary;
    private Dictionary<Vector2Int, Tile> borderTiles;

    public List<Tile> postWaterTiles;
    void Awake() {
        mapGeneration = FindObjectOfType<MapGeneration>();
        // StartPostProcess();
        mapGeneration.OnChunkGenerated += EnqueueChunkForPostProcessing;
        
        StartCoroutine(ProcessChunkQueue());
    }
    
    
    private void EnqueueChunkForPostProcessing(Vector2Int start, Dictionary<Vector2Int, Tile> tileDictionary, Dictionary<Vector2Int, Tile> borderTiles)
    {
        chunkQueue.Enqueue((start, new Dictionary<Vector2Int, Tile>(tileDictionary), new Dictionary<Vector2Int, Tile>(borderTiles)));
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

    public void PostProcessChunk(Vector2Int chunk, Dictionary<Vector2Int, Tile> localTileDict, Dictionary<Vector2Int, Tile> localBorderTiles)
    {
        int startX = chunk.x - 1;  // Расширим на 1 тайл влево
        int startY = chunk.y - 1;  // Расширим на 1 тайл вверх
        int endX = chunk.x + chunkSize + 1;  // Расширим на 1 тайл вправо
        int endY = chunk.y + chunkSize + 1;  // Расширим на 1 тайл вниз

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                List<Tile> tiles = GetTileAndBorderTiles(x, y, localTileDict);
                
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
        if (localTileDict.TryGetValue(new Vector2Int(x+1, y + 1), out foundTile))
        {
            mainAndBorderTiles.Add(foundTile);
        }
        if (localTileDict.TryGetValue(new Vector2Int(x-1, y), out foundTile))
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
        if (tiles.Count == 9 && tiles[4].name == "water_light")
        {
            ProcessWater(tiles, x, y);
        }

    }

    public void ProcessWater(List<Tile> tiles, int x, int y)
{
    

    if (tiles[5].name == "grass_light" && tiles[1].name == "grass_light" && tiles[6].name == "water_light" && tiles[3].name == "water_light" && tiles[7].name == "water_light")
    {
        //top-right angle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[7]);
    }
    if (tiles[3].name == "grass_light" && tiles[1].name == "grass_light" && tiles[5].name == "water_light" && tiles[8].name == "water_light" && tiles[7].name == "water_light")
    {
        // top-left angle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[5]);
    }
    if (tiles[7].name == "grass_light" && tiles[3].name == "grass_light" && tiles[1].name == "water_light" && tiles[2].name == "water_light" && tiles[5].name == "water_light")
    {
        // bottom-left angle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[21]);
    }
    if (tiles[7].name == "grass_light" && tiles[5].name == "grass_light" && tiles[0].name == "water_light" && tiles[3].name == "water_light" && tiles[1].name == "water_light")
    {
        // bottom-right angle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[23]);
    }

    // 0 1 2 
    // 3 4 5 
    // 6 7 8 

    // check sides

    if (tiles[1].name == "grass_light" && tiles[3].name == "water_light" && tiles[6].name == "water_light" && tiles[5].name == "water_light" && tiles[7].name == "water_light" && tiles[8].name == "water_light")
    {

        // top
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[6]);
    }
    if (tiles[7].name == "grass_light" && tiles[3].name == "water_light" && tiles[5].name == "water_light" && tiles[0].name == "water_light" && tiles[1].name == "water_light" && tiles[2].name == "water_light")
    {

        // bottom
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[22]);
    }
    if (tiles[5].name == "grass_light" && tiles[1].name == "water_light" && tiles[7].name == "water_light" && tiles[0].name == "water_light" && tiles[3].name == "water_light" && tiles[6].name == "water_light")
    {

        // right
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[15]);
    }
    if (tiles[3].name == "grass_light" && tiles[1].name == "water_light" && tiles[7].name == "water_light" && tiles[2].name == "water_light" && tiles[5].name == "water_light" && tiles[8].name == "water_light")
    {

        // left
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[13]);
    }

    // 0 1 2 
    // 3 4 5 
    // 6 7 8 

    // inner angles

    if (tiles[1].name == "water_light" && tiles[5].name == "water_light" && tiles[2].name == "grass_light" && tiles[0].name == "water_light" && tiles[3].name == "water_light" && tiles[6].name == "water_light" && tiles[7].name == "water_light" && tiles[8].name == "water_light")
    {

        // top-right (inner)
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[36]);
    }
    if (tiles[1].name == "water_light" && tiles[3].name == "water_light" && tiles[0].name == "grass_light" && tiles[2].name == "water_light" && tiles[5].name == "water_light" && tiles[6].name == "water_light" && tiles[7].name == "water_light" && tiles[8].name == "water_light")
    {

        // top-left (inner)
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[37]);
    }
    if (tiles[3].name == "water_light" && tiles[7].name == "water_light" && tiles[6].name == "grass_light" && tiles[0].name == "water_light" && tiles[2].name == "water_light" && tiles[1].name == "water_light" && tiles[5].name == "water_light" && tiles[8].name == "water_light")
    {

        // bottom-left (inner)
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[29]);
    }
    if (tiles[5].name == "water_light" && tiles[7].name == "water_light" && tiles[8].name == "grass_light" && tiles[0].name == "water_light" && tiles[1].name == "water_light" && tiles[2].name == "water_light" && tiles[3].name == "water_light" && tiles[6].name == "water_light")
    {
        // bottom-right (inner)
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[28]);
    }

    
    // 0 1 2 
    // 3 4 5 
    // 6 7 8 

    // dot 
    if (tiles[1].name == "grass_light" && tiles[5].name == "grass_light" && tiles[7].name == "grass_light" && tiles[3].name == "grass_light")
    {

        // dot
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[0]);
    }

    // 0 1 2 
    // 3 4 5 
    // 6 7 8 

    // dot with 1/2 water
    if (tiles[1].name == "grass_light" && tiles[3].name == "grass_light" && tiles[7].name == "grass_light" && tiles[5].name == "water_light")
    {

        // left-upper-bottom
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[1]);
    }


    if (tiles[1].name == "grass_light" && tiles[3].name == "grass_light" && tiles[5].name == "grass_light" && tiles[7].name == "water_light")
    {

        // right-upper-left
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[4]);
    }


    if (tiles[1].name == "grass_light" && tiles[5].name == "grass_light" && tiles[7].name == "grass_light" && tiles[3].name == "water_light")
    {

        // right-upper-bottom
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[3]);
    }


    if (tiles[3].name == "grass_light" && tiles[5].name == "grass_light" && tiles[7].name == "grass_light" && tiles[1].name == "water_light")
    {

        // right-left-bottom
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[20]);
    }


    if (tiles[1].name == "grass_light" && tiles[7].name == "grass_light" && tiles[3].name == "water_light" && tiles[5].name == "water_light")
    {

        // upper-bottom
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[2]);
    }


    if (tiles[5].name == "grass_light" && tiles[3].name == "grass_light" && tiles[7].name == "water_light" && tiles[1].name == "water_light")
    {

        // left-right
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[13]);
    }


    // 0 1 2 
    // 3 4 5 
    // 6 7 8

    // closed angles
    if (tiles[3].name == "grass_light" && tiles[1].name == "grass_light" && tiles[8].name == "grass_light" && tiles[5].name == "water_light" && tiles[7].name == "water_light")
    {

        // bottom-right 
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[8]);
    }


    if (tiles[1].name == "grass_light" && tiles[5].name == "grass_light" && tiles[6].name == "grass_light" && tiles[3].name == "water_light" && tiles[7].name == "water_light")
    {

        // bottom-left 
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[9]);
    }


    if (tiles[3].name == "grass_light" && tiles[7].name == "grass_light" && tiles[2].name == "grass_light" && tiles[1].name == "water_light" && tiles[5].name == "water_light")
    {

        // top-right
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[16]);
    }


    if (tiles[5].name == "grass_light" && tiles[7].name == "grass_light" && tiles[0].name == "grass_light" && tiles[1].name == "water_light" && tiles[3].name == "water_light")
    {

        // top-left
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[17]);
    }

    // 0 1 2 
    // 3 4 5 
    // 6 7 8


    // T-tiles
    if (tiles[3].name == "grass_light" && tiles[0].name == "grass_light" && tiles[6].name == "grass_light" && tiles[2].name == "grass_light" && tiles[8].name == "grass_light" && tiles[1].name == "water_light" && tiles[7].name == "water_light" && tiles[5].name == "water_light")
    {

        // top-right-bottom 
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[10]);
    }


    if (tiles[1].name == "grass_light" && tiles[0].name == "grass_light" && tiles[2].name == "grass_light" && tiles[6].name == "grass_light" && tiles[8].name == "grass_light" && tiles[3].name == "water_light" && tiles[5].name == "water_light" && tiles[7].name == "water_light")
    {

        // bottom-right-left
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[11]);
    }


    if (tiles[6].name == "grass_light" && tiles[7].name == "grass_light" && tiles[8].name == "grass_light" && tiles[0].name == "grass_light" && tiles[2].name == "grass_light" && tiles[3].name == "water_light" && tiles[5].name == "water_light" && tiles[1].name == "water_light")
    {

        // top-right-left
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[18]);
    }


    if (tiles[2].name == "grass_light" && tiles[5].name == "grass_light" && tiles[8].name == "grass_light" && tiles[0].name == "grass_light" && tiles[6].name == "grass_light" && tiles[3].name == "water_light" && tiles[1].name == "water_light" && tiles[7].name == "water_light")
    {

        // top-left-bottom 
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[19]);
    }

    // 0 1 2 
    // 3 4 5 
    // 6 7 8

    //1 wall 1 corner
    if (tiles[0].name == "grass_light" && tiles[3].name == "grass_light" && tiles[6].name == "grass_light" && tiles[8].name == "grass_light" && tiles[1].name == "water_light" && tiles[5].name == "water_light" && tiles[2].name == "water_light" && tiles[8].name == "water_light")
    {

        // top-right-bottom-rightTopAngle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[24]);
    }


    if (tiles[2].name == "grass_light" && tiles[5].name == "grass_light" && tiles[8].name == "grass_light" && tiles[6].name == "grass_light" && tiles[0].name == "water_light" && tiles[1].name == "water_light" && tiles[3].name == "water_light" && tiles[7].name == "water_light")
    {

        // top-right-bottom-rightTopAngle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[25]);
    }


    if (tiles[0].name == "grass_light" && tiles[1].name == "grass_light" && tiles[2].name == "grass_light" && tiles[8].name == "grass_light" && tiles[3].name == "water_light" && tiles[6].name == "water_light" && tiles[7].name == "water_light" && tiles[5].name == "water_light")
    {

        // top-right-bottom-rightTopAngle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[26]);
    }


    if (tiles[0].name == "grass_light" && tiles[1].name == "grass_light" && tiles[2].name == "grass_light" && tiles[6].name == "grass_light" && tiles[3].name == "water_light" && tiles[5].name == "water_light" && tiles[7].name == "water_light" && tiles[8].name == "water_light")
    {

        // top-right-bottom-rightTopAngle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[27]);
    }


    if (tiles[0].name == "grass_light" && tiles[3].name == "grass_light" && tiles[6].name == "grass_light" && tiles[2].name == "grass_light" && tiles[1].name == "water_light" && tiles[5].name == "water_light" && tiles[7].name == "water_light" && tiles[8].name == "water_light")
    {

        // top-right-bottom-rightTopAngle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[32]);
    }


    if (tiles[2].name == "grass_light" && tiles[5].name == "grass_light" && tiles[8].name == "grass_light" && tiles[0].name == "grass_light" && tiles[1].name == "water_light" && tiles[3].name == "water_light" && tiles[7].name == "water_light" && tiles[6].name == "water_light")
    {

        // top-right-bottom-rightTopAngle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[33]);
    }


    if (tiles[2].name == "grass_light" && tiles[6].name == "grass_light" && tiles[8].name == "grass_light" && tiles[7].name == "grass_light" && tiles[1].name == "water_light" && tiles[3].name == "water_light" && tiles[0].name == "water_light" && tiles[5].name == "water_light")
    {

        // top-right-bottom-rightTopAngle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[34]);
    }


    if (tiles[7].name == "grass_light" && tiles[6].name == "grass_light" && tiles[8].name == "grass_light" && tiles[0].name == "grass_light" && tiles[1].name == "water_light" && tiles[3].name == "water_light" && tiles[2].name == "water_light" && tiles[5].name == "water_light")
    {

        // top-right-bottom-rightTopAngle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[35]);
    }

    // 0 1 2 
    // 3 4 5 
    // 6 7 8

    //3 inner angles

    if (tiles[2].name == "grass_light" && tiles[6].name == "grass_light" && tiles[8].name == "grass_light" && tiles[1].name == "water_light" && tiles[0].name == "water_light" && tiles[3].name == "water_light" && tiles[7].name == "water_light" && tiles[5].name == "water_light")
    {

        // top-right-bottom-rightTopAngle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[30]);
    }


    if (tiles[0].name == "grass_light" && tiles[6].name == "grass_light" && tiles[8].name == "grass_light" && tiles[1].name == "water_light" && tiles[2].name == "water_light" && tiles[5].name == "water_light" && tiles[7].name == "water_light" && tiles[3].name == "water_light")
    {

        // top-right-bottom-rightTopAngle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[31]);
    }


    if (tiles[0].name == "grass_light" && tiles[2].name == "grass_light" && tiles[8].name == "grass_light" && tiles[1].name == "water_light" && tiles[5].name == "water_light" && tiles[3].name == "water_light" && tiles[7].name == "water_light" && tiles[6].name == "water_light")
    {

        // top-right-bottom-rightTopAngle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[38]);
    }


    if (tiles[0].name == "grass_light" && tiles[2].name == "grass_light" && tiles[6].name == "grass_light" && tiles[1].name == "water_light" && tiles[5].name == "water_light" && tiles[3].name == "water_light" && tiles[7].name == "water_light" && tiles[8].name == "water_light")
    {

        // top-right-bottom-rightTopAngle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[39]);
    }

    // 0 1 2 
    // 3 4 5 
    // 6 7 8

    //2 inner angles

    if (tiles[0].name == "grass_light" && tiles[2].name == "grass_light" && tiles[1].name == "water_light" && tiles[3].name == "water_light" && tiles[5].name == "water_light" && tiles[6].name == "water_light" && tiles[7].name == "water_light" && tiles[8].name == "water_light")
    {

        // top-right-bottom-rightTopAngle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[40]);
    }


    if (tiles[6].name == "grass_light" && tiles[8].name == "grass_light" && tiles[0].name == "water_light" && tiles[3].name == "water_light" && tiles[1].name == "water_light" && tiles[2].name == "water_light" && tiles[5].name == "water_light" && tiles[7].name == "water_light")
    {

        // top-right-bottom-rightTopAngle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[41]);
    }


    if (tiles[2].name == "grass_light" && tiles[8].name == "grass_light" && tiles[5].name == "water_light" && tiles[3].name == "water_light" && tiles[0].name == "water_light" && tiles[6].name == "water_light" && tiles[7].name == "water_light" && tiles[1].name == "water_light")
    {

        // top-right-bottom-rightTopAngle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[42]);
    }


    if (tiles[0].name == "grass_light" && tiles[6].name == "grass_light" && tiles[1].name == "water_light" && tiles[2].name == "water_light" && tiles[5].name == "water_light" && tiles[3].name == "water_light" && tiles[7].name == "water_light" && tiles[8].name == "water_light")
    {

        // top-right-bottom-rightTopAngle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[43]);
    }


    if (tiles[6].name == "grass_light" && tiles[2].name == "grass_light" && tiles[1].name == "water_light" && tiles[3].name == "water_light" && tiles[0].name == "water_light" && tiles[5].name == "water_light" && tiles[7].name == "water_light" && tiles[8].name == "water_light")
    {

        // top-right-bottom-rightTopAngle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[44]);
    }


    if (tiles[0].name == "grass_light" && tiles[8].name == "grass_light" && tiles[1].name == "water_light" && tiles[2].name == "water_light" && tiles[5].name == "water_light" && tiles[6].name == "water_light" && tiles[7].name == "water_light" && tiles[3].name == "water_light")
    {

        // top-right-bottom-rightTopAngle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[45]);
    }

    //4 inner angles

    if (tiles[0].name == "grass_light" && tiles[2].name == "grass_light" && tiles[6].name == "grass_light" && tiles[8].name == "grass_light" && tiles[1].name == "water_light" && tiles[3].name == "water_light" && tiles[5].name == "water_light" && tiles[7].name == "water_light")
    {

        // top-right-bottom-rightTopAngle
        tileMap.SetTile(new Vector3Int(x, y, 0), postWaterTiles[45]);
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