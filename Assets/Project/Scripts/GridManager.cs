using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Project.Scripts.Helper;
using Project.Scripts.ScriptableObject;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Scripts
{
    public class GridManager : MonoBehaviour
    {
        #region Private

        private static GridManager _instance;
        private const float XOffset = 0.646f * 1.3f;
        private const float YOffset = 0.710f * 1.3f;
        [SerializeField] private MatchChecker matchChecker;
        [SerializeField] private GameObject hexagonPrefab;
        [SerializeField] private GameObject bombPrefab;
        private List<Color> _hexColorList;

        #endregion

        #region Public

        public static GridManager Instance => _instance;
        public List<List<GameObject>> gameTable;
        public List<HexGroup> allHexGroups;
        public List<HexTile> tiles;
        public Vector2Int gridSize = new Vector2Int(8, 9);
        [HideInInspector] public bool shouldBombCreated;

        #endregion
        
        #region Unity Methods

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        // Start is called before the first frame update
        IEnumerator Start()
        {
            EventManager.Current.on1000Scored += On1000Score;

            _hexColorList = new List<Color>();
            gameTable = new List<List<GameObject>>();
            for (var i = 0; i < gridSize.x; ++i)
            {
                gameTable.Add(new List<GameObject>());
            }

            tiles.ForEach(tile =>
            {
                if (tile.type == TileType.Hexagon)
                {
                    _hexColorList.Add(tile.color);
                }
            });

            // Create game table
            CreateGrid();
            // Calculate and save All possible hex groups in game table
            allHexGroups = new HexGroupCalculator(gridSize.x, gridSize.y).GetHexGroupList();

            // Check for match before game starts and don't count them as score
            var matchedHexGroups = matchChecker.GetMatchedHexGroups();
            yield return new WaitForSeconds(0.6f);
            if (matchedHexGroups.Count > 0)
                StartCoroutine(matchChecker.OnHexGroupsMatched(matchedHexGroups, true));
        }

        private void OnDestroy()
        {
            EventManager.Current.on1000Scored -= On1000Score;
        }
        
        #endregion

        #region Hexagon Operations

        /// <summary>
        /// Creates a new hexagon on specified table coordinates and sets all hexagon data.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="withAnimation"></param>
        /// <returns>New hexagon gameObject</returns>
        public GameObject CreateHexagon(Vector2Int coordinates, bool withAnimation = false)
        {
            var destPosition = CalculatePositionFromCoordinate(coordinates);
            GameObject hexObj = Instantiate(shouldBombCreated ? bombPrefab : hexagonPrefab, destPosition,
                Quaternion.identity);
            if (withAnimation)
            {
                hexObj.transform.DOMoveY(9.4f, 0);
                hexObj.transform.DOMoveY(destPosition.y, 0.3f);
            }

            // Name the gameobject something sensible.
            hexObj.name = "Hex (" + coordinates.x + "," + coordinates.y + ")";
            hexObj.transform.SetParent(transform);

            var randomColor = _hexColorList[Random.Range(0, _hexColorList.Count)];
            hexObj.GetComponent<SpriteRenderer>().color = randomColor;
            var hexData = hexObj.GetComponent<Hex>();
            hexData.color = randomColor;
            hexData.tablePos = coordinates;
            hexData.neighbours = GetAllNeighbourHexObjects(coordinates);
            hexData.isBomb = shouldBombCreated;

            shouldBombCreated = false;

            return hexObj;
        }

        /// <summary>
        /// Moves a hexagon to specified destination coordinate and set all hexagon data for new position.
        /// </summary>
        /// <param name="sourceHex"></param>
        /// <param name="destinationCoord"></param>
        public void MoveHexagon(Hex sourceHex, Vector2Int destinationCoord)
        {
            gameTable[destinationCoord.x][destinationCoord.y] = sourceHex.gameObject;
            sourceHex.tablePos = destinationCoord;
            sourceHex.neighbours = GetAllNeighbourHexObjects(sourceHex.tablePos);
            sourceHex.ClearHexGroupList();
            // Name the gameobject something sensible.
            sourceHex.name = "Hex (" + destinationCoord.x + "," + destinationCoord.y + ")";
            sourceHex.transform.DOMove(CalculatePositionFromCoordinate(destinationCoord), 0.3f);
        }

        /// <summary>
        /// Calculates and returns world Vector3 position from given game table coordinates. 
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns>World position of given coordinates</returns>
        public Vector3 CalculatePositionFromCoordinate(Vector2Int coordinates)
        {
            float yPos = coordinates.y * YOffset;

            // Is an odd column?
            if (coordinates.x % 2 == 1)
            {
                yPos -= YOffset / 2f;
            }

            return new Vector3(coordinates.x * XOffset, yPos);
        }

        /// <summary>
        /// Creates grid which game will play on.
        /// </summary>
        private void CreateGrid()
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    gameTable[x].Add(CreateHexagon(new Vector2Int(x, y)));
                }
            }

            //Test method
            // TestNeighbour();
            // TestHexGroup();
        }

        /// <summary>
        /// Calculates all neighbour hexagons and their directions.
        /// </summary>
        /// <param name="posInTable"></param>
        /// <returns>Dictionary that holds neighbour coordinates and directions</returns>
        private Dictionary<NeighbourDirection, Vector2Int> GetAllNeighbourHexObjects(Vector2Int posInTable)
        {
            List<Vector2Int> allPossibleNeighbours = new List<Vector2Int>();
            bool isOddCol = posInTable.x % 2 == 1;

            allPossibleNeighbours.Add(posInTable + Vector2Int.up); // top
            allPossibleNeighbours.Add(posInTable +
                                      (isOddCol ? Vector2Int.left : Vector2Int.up + Vector2Int.left)); // leftTop
            allPossibleNeighbours.Add(posInTable +
                                      (isOddCol ? Vector2Int.down + Vector2Int.left : Vector2Int.left)); // leftBottom
            allPossibleNeighbours.Add(posInTable + Vector2Int.down); // bottom
            allPossibleNeighbours.Add(posInTable +
                                      (isOddCol
                                          ? Vector2Int.right + Vector2Int.down
                                          : Vector2Int.right)); // rightBottom
            allPossibleNeighbours.Add(posInTable +
                                      (isOddCol ? Vector2Int.right : Vector2Int.right + Vector2Int.up)); // rightTop

            Dictionary<NeighbourDirection, Vector2Int> result = new Dictionary<NeighbourDirection, Vector2Int>();

            var i = 0;
            foreach (var neighbour in allPossibleNeighbours)
            {
                if (neighbour.x >= 0 && neighbour.x < gridSize.x && neighbour.y >= 0 && neighbour.y < gridSize.y)
                {
                    switch (i)
                    {
                        case 0:
                            result[NeighbourDirection.TOP] = neighbour;
                            break;
                        case 1:
                            result[NeighbourDirection.LEFT_TOP] = neighbour;
                            break;
                        case 2:
                            result[NeighbourDirection.LEFT_DOWN] = neighbour;
                            break;
                        case 3:
                            result[NeighbourDirection.DOWN] = neighbour;
                            break;
                        case 4:
                            result[NeighbourDirection.RIGHT_DOWN] = neighbour;
                            break;
                        case 5:
                            result[NeighbourDirection.RIGHT_TOP] = neighbour;
                            break;
                    }
                }

                ++i;
            }

            return result;
        }

        #endregion
        
        #region Event Callbacks

        /// <summary>
        /// Called every 1000 scores.
        /// Sets shouldBombCreated for create a bomb hexagon.
        /// </summary>
        private void On1000Score()
        {
            shouldBombCreated = true;
        }

        #endregion

        #region Test Methods

        // TEST Method for neighbour list correctivity
        private void TestNeighbour()
        {
            var neighbours = gameTable[0][0].GetComponent<Hex>().neighbours;
            foreach (var neighbour in neighbours)
            {
                gameTable[neighbour.Value.x][neighbour.Value.y].GetComponent<SpriteRenderer>().color = Color.black;
            }

            neighbours = gameTable[2][3].GetComponent<Hex>().neighbours;
            foreach (var neighbour in neighbours)
            {
                gameTable[neighbour.Value.x][neighbour.Value.y].GetComponent<SpriteRenderer>().color = Color.black;
            }

            neighbours = gameTable[7][2].GetComponent<Hex>().neighbours;
            foreach (var neighbour in neighbours)
            {
                gameTable[neighbour.Value.x][neighbour.Value.y].GetComponent<SpriteRenderer>().color = Color.black;
            }

            neighbours = gameTable[6][8].GetComponent<Hex>().neighbours;
            foreach (var neighbour in neighbours)
            {
                gameTable[neighbour.Value.x][neighbour.Value.y].GetComponent<SpriteRenderer>().color = Color.black;
            }

            neighbours = gameTable[0][7].GetComponent<Hex>().neighbours;
            foreach (var neighbour in neighbours)
            {
                gameTable[neighbour.Value.x][neighbour.Value.y].GetComponent<SpriteRenderer>().color = Color.black;
            }
        }

        // TEST Method for possible Hex groups
        private void TestHexGroup()
        {
            var hexGroup = gameTable[0][0].GetComponent<Hex>().GetSelectedHexGroup();
            {
                gameTable[0][0].GetComponent<SpriteRenderer>().color = Color.black;
                gameTable[hexGroup.firstHex.x][hexGroup.firstHex.y].GetComponent<SpriteRenderer>().color = Color.white;
                gameTable[hexGroup.secondHex.x][hexGroup.secondHex.y].GetComponent<SpriteRenderer>().color =
                    Color.white;
            }

            hexGroup = gameTable[2][3].GetComponent<Hex>().GetSelectedHexGroup();

            {
                gameTable[2][3].GetComponent<SpriteRenderer>().color = Color.black;
                gameTable[hexGroup.firstHex.x][hexGroup.firstHex.y].GetComponent<SpriteRenderer>().color = Color.white;
                gameTable[hexGroup.secondHex.x][hexGroup.secondHex.y].GetComponent<SpriteRenderer>().color =
                    Color.white;
            }

            hexGroup = gameTable[7][2].GetComponent<Hex>().GetSelectedHexGroup();
            {
                gameTable[7][2].GetComponent<SpriteRenderer>().color = Color.black;
                gameTable[hexGroup.firstHex.x][hexGroup.firstHex.y].GetComponent<SpriteRenderer>().color = Color.white;
                gameTable[hexGroup.secondHex.x][hexGroup.secondHex.y].GetComponent<SpriteRenderer>().color =
                    Color.white;
            }

            hexGroup = gameTable[6][8].GetComponent<Hex>().GetSelectedHexGroup();
            {
                gameTable[6][8].GetComponent<SpriteRenderer>().color = Color.black;
                gameTable[hexGroup.firstHex.x][hexGroup.firstHex.y].GetComponent<SpriteRenderer>().color = Color.white;
                gameTable[hexGroup.secondHex.x][hexGroup.secondHex.y].GetComponent<SpriteRenderer>().color =
                    Color.white;
            }

            hexGroup = gameTable[0][7].GetComponent<Hex>().GetSelectedHexGroup();
            {
                gameTable[0][7].GetComponent<SpriteRenderer>().color = Color.black;
                gameTable[hexGroup.firstHex.x][hexGroup.firstHex.y].GetComponent<SpriteRenderer>().color = Color.white;
                gameTable[hexGroup.secondHex.x][hexGroup.secondHex.y].GetComponent<SpriteRenderer>().color =
                    Color.white;
            }
        }
        
        #endregion
    }
}