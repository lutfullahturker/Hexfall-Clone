using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts
{
    public class Hex : MonoBehaviour
    {
        #region Private

        private List<HexGroup> _allHexGroups;
        private int selectedHexGroupIndex;
        private bool isFirstAccessToHexGroupList = true;
        
        #endregion

        #region Public

        [HideInInspector] public Vector2Int tablePos;
        [HideInInspector] public Color color;
        [HideInInspector] public bool isBomb;
        public Dictionary<NeighbourDirection, Vector2Int> neighbours;

        #endregion

        /// <summary>
        /// Returns next hex group in Hex's all hex groups.
        /// </summary>
        /// <returns></returns>
        public HexGroup GetNextHexGroup()
        {
            GetAllPossibleHexGroups();
            if (selectedHexGroupIndex == _allHexGroups.Count - 1 || isFirstAccessToHexGroupList)
            {
                isFirstAccessToHexGroupList = false;
                selectedHexGroupIndex = 0;
            }
            else
            {
                selectedHexGroupIndex += 1;
            }

            return _allHexGroups[selectedHexGroupIndex];
        }

        public HexGroup GetSelectedHexGroup()
        {
            GetAllPossibleHexGroups();
            return _allHexGroups[selectedHexGroupIndex];
        }

        // Called when hex selection changed
        /// <summary>
        /// Resets Hex group selections of this hex.
        /// </summary>
        public void ResetSelectedHexGroup()
        {
            selectedHexGroupIndex = 0;
            isFirstAccessToHexGroupList = true;
        }

        /// <summary>
        /// Removes all hexGroups of this hex. Use when hex position is moved.
        /// </summary>
        public void ClearHexGroupList()
        {
            _allHexGroups = null;
        }

        /// <summary>
        /// Calculates and saves all possible hex groups from neighbour hex list. 
        /// </summary>
        private void GetAllPossibleHexGroups()
        {
            if (_allHexGroups != null)
            {
                return;
            }

            _allHexGroups = new List<HexGroup>();
            var keyList = neighbours.Keys.ToList();

            if (IsHexGroupValid(keyList, NeighbourDirection.TOP, NeighbourDirection.LEFT_TOP))
            {
                _allHexGroups.Add(new HexGroup(tablePos, neighbours[NeighbourDirection.TOP],
                    neighbours[NeighbourDirection.LEFT_TOP], HexGroup.HighlightSpriteRotation.RightTwo));
            }

            if (IsHexGroupValid(keyList, NeighbourDirection.LEFT_TOP, NeighbourDirection.LEFT_DOWN))
            {
                _allHexGroups.Add(new HexGroup(tablePos, neighbours[NeighbourDirection.LEFT_TOP],
                    neighbours[NeighbourDirection.LEFT_DOWN], HexGroup.HighlightSpriteRotation.LeftTwo));
            }

            if (IsHexGroupValid(keyList, NeighbourDirection.LEFT_DOWN, NeighbourDirection.DOWN))
            {
                _allHexGroups.Add(new HexGroup(tablePos, neighbours[NeighbourDirection.LEFT_DOWN],
                    neighbours[NeighbourDirection.DOWN], HexGroup.HighlightSpriteRotation.RightTwo));
            }

            if (IsHexGroupValid(keyList, NeighbourDirection.DOWN, NeighbourDirection.RIGHT_DOWN))
            {
                _allHexGroups.Add(new HexGroup(tablePos, neighbours[NeighbourDirection.DOWN],
                    neighbours[NeighbourDirection.RIGHT_DOWN], HexGroup.HighlightSpriteRotation.LeftTwo));
            }

            if (IsHexGroupValid(keyList, NeighbourDirection.RIGHT_DOWN, NeighbourDirection.RIGHT_TOP))
            {
                _allHexGroups.Add(new HexGroup(tablePos, neighbours[NeighbourDirection.RIGHT_DOWN],
                    neighbours[NeighbourDirection.RIGHT_TOP], HexGroup.HighlightSpriteRotation.RightTwo));
            }

            if (IsHexGroupValid(keyList, NeighbourDirection.RIGHT_TOP, NeighbourDirection.TOP))
            {
                _allHexGroups.Add(new HexGroup(tablePos, neighbours[NeighbourDirection.RIGHT_TOP],
                    neighbours[NeighbourDirection.TOP], HexGroup.HighlightSpriteRotation.LeftTwo));
            }
        }

        /// <summary>
        /// Returns true if this hex has neighbours in the given directions.
        /// </summary>
        /// <param name="keyList"></param>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private bool IsHexGroupValid(
            List<NeighbourDirection> keyList,
            NeighbourDirection first,
            NeighbourDirection second)
        {
            return keyList.Contains(first) && keyList.Contains(second);
        }
    }
}