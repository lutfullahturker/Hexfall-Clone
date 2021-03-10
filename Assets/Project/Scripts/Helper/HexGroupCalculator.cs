using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Helper
{
    /// <summary>
    /// Calculates all hex groups according to the given grid size and provides a list of HexGroup. 
    /// </summary>
    public class HexGroupCalculator
    {
        private readonly int _x;
        private readonly int _y;
        private List<HexGroup> _groups;

        public HexGroupCalculator(int colSize, int rowSize)
        {
            _x = colSize;
            _y = rowSize;
            CalculateAllHexGroupsInTable();
        }

        public List<HexGroup> GetHexGroupList()
        {
            return _groups;
        }
        
        /// <summary>
        /// Calculates all hex groups in game table depends on the sizes which are given in the constructor.
        /// </summary>
        private void CalculateAllHexGroupsInTable()
        {
            _groups = new List<HexGroup>();
            int col, row;

            // Only EVEN Columns
            for (col = 0; col < _x - 1; col += 2)
            for (row = 0; row < _y - 1; row++)
            {
                // Left side 2 hex (same column has 2 hex) 
                var first = new Vector2Int(col, row);
                var second = new Vector2Int(col, row + 1);
                var third = new Vector2Int(col + 1, row + 1);
                _groups.Add(new HexGroup(first, second, third, HexGroup.HighlightSpriteRotation.LeftTwo));
                
                // Right side 2 hex (right column has 2 hex) 
                first = new Vector2Int(col, row);
                second = new Vector2Int(col + 1, row + 1);
                third = new Vector2Int(col + 1, row);
                _groups.Add(new HexGroup(first, second, third, HexGroup.HighlightSpriteRotation.RightTwo));
            }

            // Only ODD Columns and Left side 2 hex (same column has 2 hex) 
            for (col = 1; col < _x - 1; col += 2)
            for (row = 0; row < _y - 1; row++)
            {
                var first = new Vector2Int(col, row);
                var second = new Vector2Int(col, row + 1);
                var third = new Vector2Int(col + 1, row);
                _groups.Add(new HexGroup(first, second, third, HexGroup.HighlightSpriteRotation.LeftTwo));
            }

            // Only ODD Columns and Right side 2 hex (right column has 2 hex) 
            for (col = 1; col < _x - 1; col += 2)
            for (row = 1; row < _y; row++)
            {
                var first = new Vector2Int(col, row);
                var second = new Vector2Int(col + 1, row);
                var third = new Vector2Int(col + 1, row - 1);
                _groups.Add(new HexGroup(first, second, third, HexGroup.HighlightSpriteRotation.RightTwo));
            }
        }
    }
}
