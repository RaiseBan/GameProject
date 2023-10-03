using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NatureTilemapManager : MonoBehaviour
{
    public Tilemap natureTilemap;
    public List<Tile> natureTiles; // Это массив тайлов для природы (деревья и т.д.)

    // Словарь для хранения сгенерированных тайлов природы
    public Dictionary<Vector2Int, Dictionary<Vector2Int, Tile>> savedNatureChunks = 
        new Dictionary<Vector2Int, Dictionary<Vector2Int, Tile>>();

    public Dictionary<Vector2Int, Dictionary<Vector2Int, Tile>> getSavedNatureChunks()
    {
        return savedNatureChunks;
    }

    public void GenerateNatureTiles(Vector2Int chunkPosition, int chunkSize)
    {
        // Проверяем, есть ли уже сохраненные тайлы для этого чанка
        if (savedNatureChunks.ContainsKey(chunkPosition))
        {
            return; // Если есть, ничего не делаем
        }

        Dictionary<Vector2Int, Tile> newNatureTiles = new Dictionary<Vector2Int, Tile>();
        HashSet<Vector2Int> generatedPositions = new HashSet<Vector2Int>();  // Сюда будем сохранять позиции сгенерированных тайлов
        int numNatureTiles = Random.Range(5, 15); // Количество тайлов природы для этого чанка
        int startX = chunkPosition.x * chunkSize;
        int startY = chunkPosition.y * chunkSize;
        int endX = startX + chunkSize;
        int endY = startY + chunkSize;
        int minDistance = 6; // Минимальное расстояние между тайлами природы

        for (int i = 0; i < numNatureTiles; i++)
        {
            Vector2Int pos = GetRandomNatureTilePosition(startX, endX, startY, endY, generatedPositions, minDistance);
            generatedPositions.Add(pos);

            int tile_id = Random.Range(0, natureTiles.Count);  // Просто выбираем случайный тайл из массива
            natureTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), natureTiles[tile_id]);
            newNatureTiles.Add(pos, natureTiles[tile_id]);
        }

        // Сохраняем сгенерированные тайлы природы
        savedNatureChunks.Add(chunkPosition, newNatureTiles);
    }

    public void RestoreNatureTiles(Vector2Int chunkPosition)
    {
        if (savedNatureChunks.ContainsKey(chunkPosition))
        {
            var natureTiles = savedNatureChunks[chunkPosition];
            foreach (var tileInfo in natureTiles)
            {
                natureTilemap.SetTile(new Vector3Int(tileInfo.Key.x, tileInfo.Key.y, 0), tileInfo.Value);
            }
        }
    }

    public void RemoveNatureTiles(Vector2Int chunkPosition)
    {
        if (savedNatureChunks.ContainsKey(chunkPosition))
        {
            var natureTiles = savedNatureChunks[chunkPosition];
            foreach (var tileInfo in natureTiles)
            {
                natureTilemap.SetTile(new Vector3Int(tileInfo.Key.x, tileInfo.Key.y, 0), null);
            }
        }
    }

    private Vector2Int GetRandomNatureTilePosition(int startX, int endX, int startY, int endY, HashSet<Vector2Int> existingPositions, int minDistance)
    {
        Vector2Int randomPosition;
        do
        {
            randomPosition = new Vector2Int(Random.Range(startX, endX), Random.Range(startY, endY));
        } 
        while (existingPositions.Any(pos => Vector2Int.Distance(pos, randomPosition) < minDistance));

        return randomPosition;
    }
}
