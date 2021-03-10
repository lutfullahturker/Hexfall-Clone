using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts
{
    public class MatchChecker : MonoBehaviour
    {
        #region Private
        
        private const float SHAKE_EFFECT_DURATION = 1.0f;
        private const float SCALE_DOWN_DURATION = 0.5f;
        private GameManager _gameManager;
        private List<HexGroup> Groups => GridManager.Instance.allHexGroups;
        private float EXPLOSION_DURATION => SHAKE_EFFECT_DURATION + SCALE_DOWN_DURATION + 0.1f;

        #endregion

        private void Start()
        {
            _gameManager = GetComponent<GameManager>();
        }

        /// <summary>
        /// Travels all hex groups in the game and finds all matched hex groups.
        /// </summary>
        /// <returns></returns>
        public List<HexGroup> GetMatchedHexGroups()
        {
            var table = GridManager.Instance.gameTable;
            var result = new List<HexGroup>();

            foreach (var hexGroup in Groups)
            {
                var firstGO = table[hexGroup.firstHex.x][hexGroup.firstHex.y];
                var secondGO = table[hexGroup.secondHex.x][hexGroup.secondHex.y];
                var thirdGO = table[hexGroup.mainHex.x][hexGroup.mainHex.y];

                if (thirdGO == null || secondGO == null || firstGO == null)
                {
                    Debug.Log("Null Found - " + hexGroup.mainHex + hexGroup.firstHex + hexGroup.secondHex);
                }

                if (firstGO.GetComponent<Hex>().color == secondGO.GetComponent<Hex>().color &&
                    secondGO.GetComponent<Hex>().color == thirdGO.GetComponent<Hex>().color)
                {
                    result.Add(hexGroup);
                }
            }

            return result;
        }

        /// <summary>
        /// Called when one or more matched hex groups found.
        /// Explodes matched Hexes, increases the score and refill exploded hex positions.
        /// </summary>
        /// <param name="matchedGroups"></param>
        /// <param name="isPreGameMatch">If true, animations will be faster and scores don't count</param>
        public IEnumerator OnHexGroupsMatched(List<HexGroup> matchedGroups, bool isPreGameMatch = false)
        {
            var matchedHexSet = GetMatchedHexSet(matchedGroups);
            // Explodes all matched hexes
            StartCoroutine(ExplodeMatchedHexes(matchedHexSet));
            if (isPreGameMatch)
                Time.timeScale = 2.5f;
            yield return new WaitForSeconds(EXPLOSION_DURATION);

            if (!isPreGameMatch)
            {
                _gameManager.Score += matchedHexSet.Count * 5;
            }

            // Shift the top of exploded hexes and refill their places in game table
            yield return StartCoroutine(ShiftAndRefillExplodedPlaces());

            // Check if new hexes have a match. If they have, do the same operations
            var newMatchedGroups = GetMatchedHexGroups();
            if (newMatchedGroups.Count > 0)
            {
                yield return StartCoroutine(OnHexGroupsMatched(newMatchedGroups, isPreGameMatch));
            }

            Time.timeScale = 1;
        }

        /// <summary>
        /// Shifts the top neighbour of exploded hexes and refill remaining empty places in game table.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShiftAndRefillExplodedPlaces()
        {
            var table = GridManager.Instance.gameTable;

            // Shift Cells to Exploded empty cells
            for (int x = 0; x < table.Count; x++)
            {
                var emptyCellCount = 0;
                for (int y = 0; y < table[x].Count; y++)
                {
                    GameObject hexGO = table[x][y];
                    // Check if the place is empty and increase empty count
                    if (!hexGO)
                    {
                        ++emptyCellCount;
                    }
                    // If this hex object is valid and this column has empty place until current place
                    // Shift this hex to the bottom of this column
                    else if (emptyCellCount > 0)
                    {
                        GridManager.Instance.MoveHexagon(
                            hexGO.GetComponent<Hex>(),
                            new Vector2Int(x, y - emptyCellCount));
                    }
                }

                // Refill empty top cells
                for (int i = 0; i < emptyCellCount; i++)
                {
                    var destCoord = new Vector2Int(x, GridManager.Instance.gridSize.y - 1 - i);
                    table[destCoord.x][destCoord.y] = GridManager.Instance.CreateHexagon(destCoord, true);
                }
            }

            yield return new WaitForSeconds(0.3f);
        }

        /// <summary>
        /// Used for discard same hexes and get a set
        /// </summary>
        /// <param name="matchedGroups"></param>
        /// <returns></returns>
        private HashSet<Vector2Int> GetMatchedHexSet(List<HexGroup> matchedGroups)
        {
            var result = new HashSet<Vector2Int>();

            foreach (var group in matchedGroups)
            {
                result.Add(group.mainHex);
                result.Add(group.firstHex);
                result.Add(group.secondHex);
            }

            return result;
        }

        /// <summary>
        /// Explodes and destroys with animation all hex game objects in the given matched hex set.
        /// </summary>
        /// <param name="matchedSet"></param>
        /// <returns></returns>
        private IEnumerator ExplodeMatchedHexes(HashSet<Vector2Int> matchedSet)
        {
            var table = GridManager.Instance.gameTable;

            foreach (var coordinates in matchedSet)
            {
                var hexGameObject = table[coordinates.x][coordinates.y];
                hexGameObject.transform.position += Vector3.back * 5;
                DOTween.Sequence().Append(hexGameObject.transform.DOShakeScale(SHAKE_EFFECT_DURATION))
                    .Append(hexGameObject.transform.DOScale(Vector3.zero, SCALE_DOWN_DURATION));
                Destroy(hexGameObject, EXPLOSION_DURATION);
            }

            yield break;
        }
    }
}