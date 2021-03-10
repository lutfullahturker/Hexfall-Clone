using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts
{
    public class MatchChecker : MonoBehaviour
    {
        private const float SHAKE_EFFECT_DURATION = 1.0f;
        private const float SCALE_DOWN_DURATION = 0.5f;
        private float EXPLOSION_DURATION => SHAKE_EFFECT_DURATION + SCALE_DOWN_DURATION + 0.1f;
        private List<HexGroup> Groups => GridManager.Instance.allHexGroups;
        private GameManager _gameManager;

        private void Start()
        {
            _gameManager = GetComponent<GameManager>();
        }

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

        public IEnumerator OnHexGroupsMatched(List<HexGroup> matchedGroups, bool isPreGameMatch = false)
        {
            var matchedHexSet = GetMatchedHexSet(matchedGroups);
            StartCoroutine(ExplodeMatchedHexes(matchedHexSet));
            if (isPreGameMatch)
                Time.timeScale = 2.5f;
            yield return new WaitForSeconds(EXPLOSION_DURATION);

            if (!isPreGameMatch)
            {
                _gameManager.Score += matchedHexSet.Count * 5;
            }

            yield return StartCoroutine(ShiftAndRefillExplodedPlaces());

            var newMatchedGroups = GetMatchedHexGroups();
            if (newMatchedGroups.Count > 0)
            {
                yield return StartCoroutine(OnHexGroupsMatched(newMatchedGroups, isPreGameMatch));
            }

            Time.timeScale = 1;
        }

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
                    if (!hexGO)
                    {
                        ++emptyCellCount;
                    }
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