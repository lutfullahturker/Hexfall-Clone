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

        /// <summary>
        /// Starts rotate operation and checks for matched hexagons after rotation.
        /// </summary>
        /// <param name="group">Hexagon group which will be rotated</param>
        /// <param name="direction">Direction of rotation. Clockwise or CounterClockwise</param>
        /// <returns></returns>
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
                EventManager.Current.MoveCompleted();
                yield break;
            }
        }

        /// <summary>
        /// Rotates the given hex group in counter clockwise.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
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
            
            GridManager.Instance.MoveHexagon(aGameObject.GetComponent<Hex>(), b);
            GridManager.Instance.MoveHexagon(bGameObject.GetComponent<Hex>(), c);
            GridManager.Instance.MoveHexagon(cGameObject.GetComponent<Hex>(), a);

            yield return new WaitForSeconds(rotateDuration);
        }

        /// <summary>
        /// Rotates the given hex group in clockwise.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
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

            GridManager.Instance.MoveHexagon(aGameObject.GetComponent<Hex>(), c);
            GridManager.Instance.MoveHexagon(bGameObject.GetComponent<Hex>(), a);
            GridManager.Instance.MoveHexagon(cGameObject.GetComponent<Hex>(), b);

            yield return new WaitForSeconds(rotateDuration);
        }
    }
}