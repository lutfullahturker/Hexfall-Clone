using UnityEngine;
using UnityEngine.Tilemaps;

namespace Project.Scripts.ScriptableObject
{
    [CreateAssetMenu(fileName = "New Hex Tile")]
    public class HexTile : UnityEngine.ScriptableObject
    {
        public TileType type;
        public Color color = Color.white;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }

    public enum TileType
    {
        Hexagon,
        Bomb,
        Diamond
    }
}
