using System.Collections;
using UnityEngine;

namespace Project.Scripts
{
    /// <summary>
    /// Holds 3 hex coordinates and their directions
    /// </summary>
    public class HexGroup
    {
        public Vector2Int mainHex;
        public Vector2Int firstHex;
        public Vector2Int secondHex;
        public HighlightSpriteRotation highlightSpriteRotation;
        
        public enum RotateDirection
        {
            Clockwise,
            CounterClockwise
        }

        public enum HighlightSpriteRotation
        {
            LeftTwo,
            RightTwo
        }
        
        public HexGroup(Vector2Int _mainHex, Vector2Int _firstHex, Vector2Int _secondHex, HighlightSpriteRotation _highlightSpriteRotation)
        {
            highlightSpriteRotation = _highlightSpriteRotation;
            mainHex = _mainHex;
            firstHex = _firstHex;
            secondHex = _secondHex;
        }

        /// <summary>
        /// Returns world position of center of this hexGroup.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCenterPositionOfGroup()
        {
            var mainHexPosition = GridManager.Instance.gameTable[mainHex.x][mainHex.y].transform.position;
            var firstHexPosition = GridManager.Instance.gameTable[firstHex.x][firstHex.y].transform.position;
            var secondHexPosition = GridManager.Instance.gameTable[secondHex.x][secondHex.y].transform.position;

            var result = Vector3.zero;
            result += mainHexPosition;
            result += firstHexPosition;
            result += secondHexPosition;
            
            // Debug.Log(result/3.0f);

            return result / 3;
        }
    }
}
