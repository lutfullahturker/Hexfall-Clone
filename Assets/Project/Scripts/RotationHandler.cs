using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts
{
    public class RotationHandler : MonoBehaviour
    {
        private MatchChecker _matchChecker;

        // Start is called before the first frame update
        void Start()
        {
            _matchChecker = gameObject.GetComponent<MatchChecker>();
        }

        public IEnumerator Rotate(HexGroup group, HexGroup.RotateDirection direction)
        {
            for (int i = 0; i < 3; i++)
            {
                yield return StartCoroutine(direction == HexGroup.RotateDirection.Clockwise
                    ? RotateClockwise(group)
                    : RotateCounterClockwise(group));

                // check for match
                var matchedHexGroups = _matchChecker.GetMatchedHexGroups();
                if (matchedHexGroups.Count <= 0) continue;

                yield return StartCoroutine(_matchChecker.OnHexGroupsMatched(matchedHexGroups));
                yield break;
            }
        }

        private IEnumerator RotateCounterClockwise(HexGroup group)
        {
            const float rotateDuration = 0.3f;
            var table = GridManager.Instance.gameTable;
            var a = group.mainHex;
            var b = group.firstHex;
            var c = group.secondHex;

            var aGameObject = table[a.x][a.y];
            var bGameObject = table[b.x][b.y];
            var cGameObject = table[c.x][c.y];
            
            GridManager.Instance.MoveHexagonToEmptyCell(aGameObject.GetComponent<Hex>(), b);
            GridManager.Instance.MoveHexagonToEmptyCell(bGameObject.GetComponent<Hex>(), c);
            GridManager.Instance.MoveHexagonToEmptyCell(cGameObject.GetComponent<Hex>(), a);

            yield return new WaitForSeconds(rotateDuration);
        }

        private IEnumerator RotateClockwise(HexGroup group)
        {
            const float rotateDuration = 0.3f;
            var table = GridManager.Instance.gameTable;
            var a = group.mainHex;
            var b = group.firstHex;
            var c = group.secondHex;

            var aGameObject = table[a.x][a.y];
            var bGameObject = table[b.x][b.y];
            var cGameObject = table[c.x][c.y];

            GridManager.Instance.MoveHexagonToEmptyCell(aGameObject.GetComponent<Hex>(), c);
            GridManager.Instance.MoveHexagonToEmptyCell(bGameObject.GetComponent<Hex>(), a);
            GridManager.Instance.MoveHexagonToEmptyCell(cGameObject.GetComponent<Hex>(), b);

            yield return new WaitForSeconds(rotateDuration);
        }
    }
}