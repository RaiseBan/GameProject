using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map
{
    public class ChunkInfo
    {
        public Vector2Int Position { get; set; }
        public bool IsGenerated { get; set; }
        public Dictionary<Vector2Int, Tile> GeneratedNatureTiles { get; set; }
    }
}