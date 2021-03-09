using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Project.Scripts.Helper;
using Project.Scripts.ScriptableObject;
using UnityEngine;

namespace Project.Scripts
{
    public class GridManager : MonoBehaviour
    {
        private static GridManager _instance;
        public static GridManager Instance => _instance;

        public List<List<GameObject>> gameTable;
        public List<HexGroup> allHexGroups;
        public List<HexTile> tiles;
        public Vector2Int gridSize = new Vector2Int(8, 9);
        [SerializeField] private GameObject hexagonPrefab;
        [SerializeField] private GameObject bombPrefab;

        private readonly float xOffset = 0.646f * 1.3f;
        private readonly float yOffset = 0.710f * 1.3f;
        private List<Color> hexColorList;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            } else {
                _instance = this;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            hexColorList = new List<Color>();
            gameTable = new List<List<GameObject>>();
            for (var i = 0; i < gridSize.x; ++i)
            {
                gameTable.Add(new List<GameObject>());
            }

            tiles.ForEach(tile =>
            {
                if (tile.type == TileType.Hexagon)
                {
                    hexColorList.Add(tile.color);
                }
            });

            CreateGrid();
            allHexGroups = new HexGroupCalculator(gridSize.x,gridSize.y).GetHexGroupList();
        }

        public GameObject CreateHexagon(Vector2Int coordinates, bool withAnimation = false)
        {
            List<HexTile> hexagonList = tiles.FindAll(tile => tile.type == TileType.Hexagon);
            var destPosition = CalculatePositionFromCoordinate(coordinates);
            GameObject hexObj = Instantiate(hexagonPrefab, destPosition , Quaternion.identity);
            if (withAnimation)
            {
                hexObj.transform.DOMoveY(9.4f,0);
                hexObj.transform.DOMoveY(destPosition.y, 0.3f);
            }
            // Name the gameobject something sensible.
            hexObj.name = "Hex (" + coordinates.x + "," + coordinates.y + ")";
            hexObj.transform.SetParent(transform);

            var randomColoredHex = hexagonList[Random.Range(0, hexagonList.Count)];
            hexObj.GetComponent<SpriteRenderer>().color = randomColoredHex.color;
            var hexData = hexObj.GetComponent<Hex>();
            hexData.color = randomColoredHex.color;
            hexData.tablePos = coordinates;
            hexData.neighbours = GetAllNeighbourHexObjects(coordinates);

            return hexObj;
        }
        
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

        public Vector3 CalculatePositionFromCoordinate(Vector2Int coordinates)
        {
            float yPos = coordinates.y * yOffset;

            // Is an odd column?
            if (coordinates.x % 2 == 1)
            {
                yPos -= yOffset / 2f;
            }

            return new Vector3(coordinates.x * xOffset, yPos);
        }

        public GameObject GetHexGO(Vector2Int coord)
        {
            return gameTable[coord.x][coord.y];
        }
        
        public GameObject GetHexGO(int x, int y)
        {
            return gameTable[x][y];
        }

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

        private HexTile GetRandomColorHexagon(Vector3Int pos)
        {
            List<Color> disabledColors = new List<Color>();


            var maxPos = new Vector2Int(gridSize.y - 1, gridSize.x - 1);
            return null;
        }

        private Dictionary<NeighbourDirection, Vector2Int> GetAllNeighbourHexObjects(Vector2Int posInTable)
        {
            List<Vector2Int> allPossibleNeighbours = new List<Vector2Int>();
            bool isOddCol = posInTable.x % 2 == 1;

            allPossibleNeighbours.Add(posInTable + Vector2Int.up); // top
            allPossibleNeighbours.Add(posInTable + (isOddCol ? Vector2Int.left: Vector2Int.up + Vector2Int.left)); // leftTop
            allPossibleNeighbours.Add(posInTable + (isOddCol ? Vector2Int.down + Vector2Int.left : Vector2Int.left)); // leftBottom
            allPossibleNeighbours.Add(posInTable + Vector2Int.down); // bottom
            allPossibleNeighbours.Add(posInTable + (isOddCol ? Vector2Int.right + Vector2Int.down : Vector2Int.right)); // rightBottom
            allPossibleNeighbours.Add(posInTable + (isOddCol ? Vector2Int.right : Vector2Int.right + Vector2Int.up)); // rightTop

            Dictionary<NeighbourDirection, Vector2Int> result = new Dictionary<NeighbourDirection, Vector2Int> ();

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
                gameTable[hexGroup.secondHex.x][hexGroup.secondHex.y].GetComponent<SpriteRenderer>().color = Color.white;
            }
            
            hexGroup = gameTable[2][3].GetComponent<Hex>().GetSelectedHexGroup();

            {
                gameTable[2][3].GetComponent<SpriteRenderer>().color = Color.black;
                gameTable[hexGroup.firstHex.x][hexGroup.firstHex.y].GetComponent<SpriteRenderer>().color = Color.white;
                gameTable[hexGroup.secondHex.x][hexGroup.secondHex.y].GetComponent<SpriteRenderer>().color = Color.white;
            }
            
            hexGroup = gameTable[7][2].GetComponent<Hex>().GetSelectedHexGroup();
            {
                gameTable[7][2].GetComponent<SpriteRenderer>().color = Color.black;
                gameTable[hexGroup.firstHex.x][hexGroup.firstHex.y].GetComponent<SpriteRenderer>().color = Color.white;
                gameTable[hexGroup.secondHex.x][hexGroup.secondHex.y].GetComponent<SpriteRenderer>().color = Color.white;
            }
            
            hexGroup = gameTable[6][8].GetComponent<Hex>().GetSelectedHexGroup();
            {
                gameTable[6][8].GetComponent<SpriteRenderer>().color = Color.black;
                gameTable[hexGroup.firstHex.x][hexGroup.firstHex.y].GetComponent<SpriteRenderer>().color = Color.white;
                gameTable[hexGroup.secondHex.x][hexGroup.secondHex.y].GetComponent<SpriteRenderer>().color = Color.white;
            }
            
            hexGroup = gameTable[0][7].GetComponent<Hex>().GetSelectedHexGroup();
            {
                gameTable[0][7].GetComponent<SpriteRenderer>().color = Color.black;
                gameTable[hexGroup.firstHex.x][hexGroup.firstHex.y].GetComponent<SpriteRenderer>().color = Color.white;
                gameTable[hexGroup.secondHex.x][hexGroup.secondHex.y].GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }
}