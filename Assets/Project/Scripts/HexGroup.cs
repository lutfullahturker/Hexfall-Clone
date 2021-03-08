using UnityEngine;

namespace Project.Scripts
{
    public class HexGroup
    {
        public Vector2Int mainHex;
        public Vector2Int firstHex;
        public Vector2Int secondHex;
        public Rotation rotation;
        
        public enum RotateDirection
        {
            Clockwise,
            CounterClockwise
        }

        public enum Rotation
        {
            LeftTwo,
            RightTwo
        }
        
        public HexGroup(Vector2Int _mainHex, Vector2Int _firstHex, Vector2Int _secondHex, Rotation _rotation)
        {
            rotation = _rotation;
            mainHex = _mainHex;
            firstHex = _firstHex;
            secondHex = _secondHex;
        }

        public Vector3 GetCenterPositionOfGroup()
        {
            var mainHexPosition = GridCreator.gameTable[mainHex.x][mainHex.y].transform.position;
            var firstHexPosition = GridCreator.gameTable[firstHex.x][firstHex.y].transform.position;
            var secondHexPosition = GridCreator.gameTable[secondHex.x][secondHex.y].transform.position;

            var result = Vector3.zero;
            result += mainHexPosition;
            result += firstHexPosition;
            result += secondHexPosition;
            
            // Debug.Log(result/3.0f);

            return result / 3;
        }
    }
}
