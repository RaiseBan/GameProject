using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NatureTilemapManager : MonoBehaviour
{
    public Tilemap mainTilemap;
    public List<Tile> waterTiles;
    public GameObject treePrefab; // Префаб вашего дерева
    public Dictionary<Vector2Int, List<GameObject>> savedNatureTrees = 
        new Dictionary<Vector2Int, List<GameObject>>(); // Словарь для хранения сгенерированных деревьев

    public Dictionary<Vector2Int, List<GameObject>> getSavedNatureTrees()
    {
        return savedNatureTrees;
    }
    
    private bool IsWaterTile(Vector2Int position)
    {
        Tile tile = mainTilemap.GetTile<Tile>(new Vector3Int(position.x, position.y, 0));
        foreach (var VARIABLE in waterTiles)
        {
            if (tile == VARIABLE)
            {
                return true;
            }
        }
        return false;
    }
    private bool IsNearWater(Vector2Int position, int radius = 3)
    {
        for (int x = position.x - radius; x <= position.x + radius; x++)
        {
            for (int y = position.y - radius; y <= position.y + radius; y++)
            {
                if (IsWaterTile(new Vector2Int(x, y)))
                {
                    return true;
                }
            }
        }
        return false;
    }

    

    public void GenerateNatureTrees(Vector2Int chunkPosition, int chunkSize)
    {
        if (savedNatureTrees.ContainsKey(chunkPosition))
        {
            return;
        }

        List<GameObject> newTrees = new List<GameObject>();
        HashSet<Vector2Int> generatedPositions = new HashSet<Vector2Int>();
        int numTrees = Random.Range(5, 15);
        int startX = chunkPosition.x * chunkSize;
        int startY = chunkPosition.y * chunkSize;
        int endX = startX + chunkSize;
        int endY = startY + chunkSize;
        int minDistance = 15;

        for (int i = 0; i < numTrees; i++)
        {
            Vector2Int pos = GetRandomNatureTilePosition(startX, endX, startY, endY, generatedPositions, minDistance);
            
            if (pos == new Vector2Int(-1, -1)) // Если позиция невалидна, пропускаем эту итерацию
            {
                continue;
            }

            generatedPositions.Add(pos);

            GameObject treeInstance = Instantiate(treePrefab, new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Quaternion.identity);
            newTrees.Add(treeInstance);
        }

        savedNatureTrees.Add(chunkPosition, newTrees);
    }

    public void RestoreNatureTrees(Vector2Int chunkPosition)
    {
        if (savedNatureTrees.ContainsKey(chunkPosition))
        {
            foreach (var tree in savedNatureTrees[chunkPosition])
            {
                tree.SetActive(true);
            }
        }
    }

    public void RemoveNatureTrees(Vector2Int chunkPosition)
    {
        if (savedNatureTrees.ContainsKey(chunkPosition))
        {
            foreach (var tree in savedNatureTrees[chunkPosition])
            {
                tree.SetActive(false);
            }
        }
    }

    private Vector2Int GetRandomNatureTilePosition(int startX, int endX, int startY, int endY, HashSet<Vector2Int> existingPositions, int minDistance)
    {
        int bufferZone = 2; 
        startX += bufferZone;
        startY += bufferZone;
        endX -= bufferZone;
        endY -= bufferZone;

        Vector2Int randomPosition;
        int maxAttempts = 100;
        int currentAttempt = 0;

        do
        {
            randomPosition = new Vector2Int(Random.Range(startX, endX), Random.Range(startY, endY));
            currentAttempt++;
        } 
        while ((IsPositionTooCloseToExistingTrees(randomPosition, existingPositions, minDistance) 
                || IsWaterTile(randomPosition) || IsNearWater(randomPosition)) && currentAttempt < maxAttempts);


        if (currentAttempt == maxAttempts)
        {
            return new Vector2Int(-1, -1); 
        }

        return randomPosition;
    }

    private bool IsPositionTooCloseToExistingTrees(Vector2Int newPosition, HashSet<Vector2Int> existingPositions, int minDistance)
    {
        foreach (var pos in existingPositions)
        {
            if (Vector2Int.Distance(pos, newPosition) < minDistance)
            {
                // Если новая позиция близка к существующему дереву, учитывая размер дерева
                return true;
            }
        }
        return false;
    }
}
