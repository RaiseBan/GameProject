using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class NatureTilemapManager : MonoBehaviour
{
    public GameObject treeParent; // GameObject для хранения всех деревьев
    public GameObject stoneParent;
    public List<GameObject> stonesPrefabs;
    public List<GameObject> cuprumPrefabs;
    public List<GameObject> ironPrefabs;
    public List<GameObject> coalPrefabs;
    public List<List<GameObject>> minerals;
    public List<GameObject> treesPrefabs;
    
    public Tilemap mainTilemap;
    public List<Tile> waterTiles;
    public GameObject treePrefab; // Префаб вашего дерева
    public Dictionary<Vector2Int, List<GameObject>> savedNatureTrees = 
        new Dictionary<Vector2Int, List<GameObject>>(); // Словарь для хранения сгенерированных деревьев
    public GameObject stonePrefab; // Префаб вашего камня
    public Dictionary<Vector2Int, List<GameObject>> savedNatureStones =
        new Dictionary<Vector2Int, List<GameObject>>(); // Словарь для хранения сгенерированных камней

    public Dictionary<Vector2Int, List<GameObject>> getSavedNatureRocks()
    {
        return savedNatureStones;
    }
    public Dictionary<Vector2Int, List<GameObject>> getSavedNatureTrees()
    {
        return savedNatureTrees;
    }
    
    void Start()
    {
        stoneParent = new GameObject("Stones");
        treeParent = new GameObject("Trees"); // Создаем новый GameObject с именем "Trees"
        // minerals.Add(stonesPrefabs);
        // minerals.Add(stonesPrefabs);
        // minerals.Add(stonesPrefabs);
        // minerals.Add(stonesPrefabs);
        //TODO: ^^^^^
        
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

    

    public void GenerateNatureObjects(Vector2Int chunkPosition, int chunkSize)
    {
        // Debug.Log("GenerateNatureObjects called for chunk: " + chunkPosition);
        if (savedNatureTrees.ContainsKey(chunkPosition))
        {
            return;
        }

        List<GameObject> newTrees = new List<GameObject>();
        HashSet<Vector2Int> generatedPositions = new HashSet<Vector2Int>();
        int numTrees = Random.Range(5, 10);
        int startX = chunkPosition.x * chunkSize;
        int startY = chunkPosition.y * chunkSize;
        int endX = startX + chunkSize;
        int endY = startY + chunkSize;
        int minDistance = 15;
        int rockDistance = 10;

        for (int i = 0; i < numTrees; i++)
        {
            Vector2Int pos = GetRandomNatureTilePosition(startX, endX, startY, endY, generatedPositions, minDistance);
            
            if (pos == new Vector2Int(-1, -1)) // Если позиция невалидна, пропускаем эту итерацию
            {
                continue;
            }

            generatedPositions.Add(pos);

            GameObject treeInstance = Instantiate(GetRandomPrefab("tree"), new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Quaternion.identity);
            treeInstance.transform.SetParent(treeParent.transform); // Установить treeParent в качестве родителя
            newTrees.Add(treeInstance);
        }

        savedNatureTrees.Add(chunkPosition, newTrees);
        
        List<GameObject> newStones = new List<GameObject>();
        int numStones = Random.Range(3, 10); // Выберите количество камней в зависимости от вашего дизайна уровня
        // Debug.Log("after ");
        for (int i = 0; i < numStones; i++)
        {
            Vector2Int pos = GetRandomNatureTilePosition(startX, endX, startY, endY, generatedPositions, rockDistance);

            if (pos == new Vector2Int(-1, -1))
            {
                continue;
            }

            generatedPositions.Add(pos);

            GameObject stoneInstance = Instantiate(GetRandomPrefab("stone"), new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Quaternion.identity);
            stoneInstance.transform.SetParent(stoneParent.transform);
            newStones.Add(stoneInstance);
            // Debug.Log("generated");
        }

        savedNatureStones.Add(chunkPosition, newStones);
    }

    public GameObject GetRandomPrefab(String type)
    {
        if (type == "tree")
        {
            int num = Random.Range(0, treesPrefabs.Count);
            return treesPrefabs[num];
        }
        if (type == "stone")
        {
            //TODO: Сделать рандом
            int rockNumber = randomNinerals.GetRandomStoneType(); // TODO: взависимости от местности, уровня или других зарактеристик будет передавать какой-то коэффициент
            List<GameObject> currentMineral = minerals[rockNumber];
            int num = Random.Range(0, currentMineral.Count);
            return stonesPrefabs[num];
        }

        return null;
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
    public void RestoreNatureStones(Vector2Int chunkPosition)
    {
        if (savedNatureStones.ContainsKey(chunkPosition))
        {
            foreach (var stone in savedNatureStones[chunkPosition])
            {
                stone.SetActive(true);
            }
        }
    }
    public void RemoveNatureStones(Vector2Int chunkPosition)
    {
        if (savedNatureStones.ContainsKey(chunkPosition))
        {
            foreach (var stone in savedNatureStones[chunkPosition])
            {
                stone.SetActive(false);
            }
        }
    }
}
