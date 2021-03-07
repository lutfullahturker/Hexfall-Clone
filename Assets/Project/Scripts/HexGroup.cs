using UnityEngine;

namespace Project.Scripts
{
    public class HexGroup
    {
        public Vector2Int mainHex;
        public Vector2Int firstHex;
        public Vector2Int secondHex;
        
        public HexGroup(Vector2Int _mainHex, Vector2Int _firstHex, Vector2Int _secondHex)
        {
            mainHex = _mainHex;
            firstHex = _firstHex;
            secondHex = _secondHex;
        }
    }
}
