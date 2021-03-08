using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts
{
    public class Hex : MonoBehaviour
    {
        [HideInInspector] public Vector2Int tablePos;
        [HideInInspector] public Color color;
        public Dictionary<NeighbourDirection, Vector2Int> neighbours;

        private List<HexGroup> _allHexGroups;
        private int selectedHexGroupIndex;
        private bool isFirstAccessToHexGroupList = true;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
        
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

        public void ResetSelectedHexGroup()
        {
            selectedHexGroupIndex = 0;
            isFirstAccessToHexGroupList = true;
        }
        
        public IEnumerator Rotate(HexGroup.RotateDirection direction)
        {
            for (int i = 0; i < 3; i++)
            {
                yield return StartCoroutine(direction == HexGroup.RotateDirection.Clockwise
                    ? RotateClockwise()
                    : RotateCounterClockwise());
                
                // check for match
                // if match successfull - yield break
            }
        }

        private IEnumerator RotateCounterClockwise()
        {
            const float rotateDuration = 0.3f;
            var table = GridCreator.gameTable;
            var a = GetSelectedHexGroup().mainHex;
            var b = GetSelectedHexGroup().firstHex;
            var c = GetSelectedHexGroup().secondHex;
            var positionOfA = table[a.x][a.y].transform.position;

            table[a.x][a.y].transform.DOMove(table[b.x][b.y].transform.position, rotateDuration);
            table[b.x][b.y].transform.DOMove(table[c.x][c.y].transform.position, rotateDuration);
            table[c.x][c.y].transform.DOMove(positionOfA, rotateDuration);

            yield return new WaitForSeconds(rotateDuration);
        }

        private IEnumerator RotateClockwise()
        {
            const float rotateDuration = 0.3f;
            var table = GridCreator.gameTable;
            var a = GetSelectedHexGroup().mainHex;
            var b = GetSelectedHexGroup().firstHex;
            var c = GetSelectedHexGroup().secondHex;
            var positionOfA = table[a.x][a.y].transform.position;

            table[a.x][a.y].transform.DOMove(table[c.x][c.y].transform.position, rotateDuration);
            table[c.x][c.y].transform.DOMove(table[b.x][b.y].transform.position, rotateDuration);
            table[b.x][b.y].transform.DOMove(positionOfA, rotateDuration);

            yield return new WaitForSeconds(rotateDuration);
        }
        
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

        private bool IsHexGroupValid(
            List<NeighbourDirection> keyList,
            NeighbourDirection first,
            NeighbourDirection second)
        {
            return keyList.Contains(first) && keyList.Contains(second);
        }
    }
}