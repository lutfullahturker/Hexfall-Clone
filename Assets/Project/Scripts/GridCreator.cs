using System.Collections.Generic;
using Project.Scripts.ScriptableObject;
using UnityEngine;

namespace Project.Scripts
{
    public class GridCreator : MonoBehaviour
    {
        [SerializeField] private GameObject hexagonPrefab;
        [SerializeField] private GameObject bombPrefab;
        public List<HexTile> tiles;
        public Vector2Int gridSize = new Vector2Int(8, 9);

        private List<Color> hexColorList;
        private List<List<GameObject>> gameTable;

        float xOffset = 0.646f;
        float yOffset = 0.710f;

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
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void CreateGrid()
        {
            List<HexTile> hexagonList = tiles.FindAll(tile => tile.type == TileType.Hexagon);

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    float yPos = y * yOffset;

                    // Is an odd column?
                    if (x % 2 == 1)
                    {
                        yPos -= yOffset / 2f;
                    }

                    GameObject hexObj = Instantiate(hexagonPrefab, new Vector3(x * xOffset, yPos), Quaternion.identity);

                    // Name the gameobject something sensible.
                    hexObj.name = "Hex (" + x + "," + y + ")";
                    hexObj.transform.SetParent(transform);

                    var randomColoredHex = hexagonList[Random.Range(0, hexagonList.Count)];
                    hexObj.GetComponent<SpriteRenderer>().color = randomColoredHex.color;
                    gameTable[x].Add(hexObj);
                    var hexData = hexObj.GetComponent<Hex>();
                    hexData.color = randomColoredHex.color;
                }
            }
            FillAllHexData();
            transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            Vector3 centerPos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10));
            gameObject.transform.position = centerPos;
        }

        private void FillAllHexData()
        {
            for (var i = 0; i < gameTable.Count; ++i)
            {
                for (int j = 0; j < gameTable[i].Count; j++)
                {
                    var hexData = gameTable[i][j].GetComponent<Hex>();
                    hexData.tablePos = new Vector2Int(i, j);
                    hexData.neighbours = GetAllNeighbourHexObjects(hexData.tablePos);
                }
            }
            
            //Test method
            // TestNeighbour();
        }

        private HexTile GetRandomColorHexagon(Vector3Int pos)
        {
            List<Color> disabledColors = new List<Color>();


            var maxPos = new Vector2Int(gridSize.y - 1, gridSize.x - 1);
            return null;
        }

        private List<GameObject> GetAllNeighbourHexObjects(Vector2Int posInTable)
        {
            List<Vector2Int> allPossibleNeighbours = new List<Vector2Int>();
            bool isOddCol = posInTable.x % 2 == 1;

            allPossibleNeighbours.Add(posInTable + Vector2Int.up); // top
            allPossibleNeighbours.Add(posInTable + (isOddCol ? Vector2Int.left: Vector2Int.up + Vector2Int.left)); // leftTop
            allPossibleNeighbours.Add(posInTable + (isOddCol ? Vector2Int.down + Vector2Int.left : Vector2Int.left)); // leftBottom
            allPossibleNeighbours.Add(posInTable + Vector2Int.down); // bottom
            allPossibleNeighbours.Add(posInTable + (isOddCol ? Vector2Int.right + Vector2Int.down : Vector2Int.right)); // rightBottom
            allPossibleNeighbours.Add(posInTable + (isOddCol ? Vector2Int.right : Vector2Int.right + Vector2Int.up)); // rightTop

            List<GameObject> result = new List<GameObject>();

            foreach (var neighbour in allPossibleNeighbours)
            {
                if (neighbour.x >= 0 && neighbour.x < gridSize.x && neighbour.y >= 0 && neighbour.y < gridSize.y)
                {
                    result.Add(gameTable[neighbour.x][neighbour.y]);
                }
            }

            return result;
        }
        
        // TEST Method for neighbour list correctivity
        private void TestNeighbour()
        {
            var neighbours = gameTable[0][0].GetComponent<Hex>().neighbours;
            foreach (var neighbour in neighbours)
            {
                neighbour.GetComponent<SpriteRenderer>().color = Color.black;
            }
            
            neighbours = gameTable[2][3].GetComponent<Hex>().neighbours;
            foreach (var neighbour in neighbours)
            {
                neighbour.GetComponent<SpriteRenderer>().color = Color.black;
            }
            
            neighbours = gameTable[7][2].GetComponent<Hex>().neighbours;
            foreach (var neighbour in neighbours)
            {
                neighbour.GetComponent<SpriteRenderer>().color = Color.black;
            }
            
            neighbours = gameTable[6][8].GetComponent<Hex>().neighbours;
            foreach (var neighbour in neighbours)
            {
                neighbour.GetComponent<SpriteRenderer>().color = Color.black;
            }
            
            neighbours = gameTable[0][7].GetComponent<Hex>().neighbours;
            foreach (var neighbour in neighbours)
            {
                neighbour.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
    }
}