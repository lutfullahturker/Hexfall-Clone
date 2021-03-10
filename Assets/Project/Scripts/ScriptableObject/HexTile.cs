using UnityEngine;
using UnityEngine.Tilemaps;

namespace Project.Scripts.ScriptableObject
{
    [CreateAssetMenu(fileName = "New Hex Tile")]
    public class HexTile : UnityEngine.ScriptableObject
    {
        public TileType type;
        public Color color = Color.white;
    }

    public enum TileType
    {
        Hexagon,
        Bomb,
        Diamond
    }
}
