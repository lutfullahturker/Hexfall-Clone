using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.Scripts
{
    public class Hex : MonoBehaviour
    {
        [HideInInspector] public Vector2Int tablePos;

        [HideInInspector] public Color color;

        public Dictionary<NeighbourDirection, Vector2Int> neighbours;

        private List<HexGroup> _allHexGroups;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public List<HexGroup> GetAllPossibleHexGroups()
        {
            if (_allHexGroups != null)
            {
                return _allHexGroups;
            }

            _allHexGroups = new List<HexGroup>();
            var keyList = neighbours.Keys.ToList();

            if (IsHexGroupValid(keyList, NeighbourDirection.TOP, NeighbourDirection.LEFT_TOP))
            {
                _allHexGroups.Add(new HexGroup(tablePos, neighbours[NeighbourDirection.TOP],
                    neighbours[NeighbourDirection.LEFT_TOP]));
            }

            if (IsHexGroupValid(keyList, NeighbourDirection.LEFT_TOP, NeighbourDirection.LEFT_DOWN))
            {
                _allHexGroups.Add(new HexGroup(tablePos, neighbours[NeighbourDirection.LEFT_TOP],
                    neighbours[NeighbourDirection.LEFT_DOWN]));
            }

            if (IsHexGroupValid(keyList, NeighbourDirection.LEFT_DOWN, NeighbourDirection.DOWN))
            {
                _allHexGroups.Add(new HexGroup(tablePos, neighbours[NeighbourDirection.LEFT_DOWN],
                    neighbours[NeighbourDirection.DOWN]));
            }

            if (IsHexGroupValid(keyList, NeighbourDirection.DOWN, NeighbourDirection.RIGHT_DOWN))
            {
                _allHexGroups.Add(new HexGroup(tablePos, neighbours[NeighbourDirection.DOWN],
                    neighbours[NeighbourDirection.RIGHT_DOWN]));
            }

            if (IsHexGroupValid(keyList, NeighbourDirection.RIGHT_DOWN, NeighbourDirection.RIGHT_TOP))
            {
                _allHexGroups.Add(new HexGroup(tablePos, neighbours[NeighbourDirection.RIGHT_DOWN],
                    neighbours[NeighbourDirection.RIGHT_TOP]));
            }

            if (IsHexGroupValid(keyList, NeighbourDirection.RIGHT_TOP, NeighbourDirection.TOP))
            {
                _allHexGroups.Add(new HexGroup(tablePos, neighbours[NeighbourDirection.RIGHT_TOP],
                    neighbours[NeighbourDirection.TOP]));
            }

            return _allHexGroups;
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