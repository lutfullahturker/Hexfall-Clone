using System.Collections.Generic;
using System.Linq;
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
            var hexGroups = gameTable[0][0].GetComponent<Hex>().GetAllPossibleHexGroups();

            var group = hexGroups.First();
            {
                gameTable[0][0].GetComponent<SpriteRenderer>().color = Color.black;
                gameTable[group.firstHex.x][group.firstHex.y].GetComponent<SpriteRenderer>().color = Color.white;
                gameTable[group.secondHex.x][group.secondHex.y].GetComponent<SpriteRenderer>().color = Color.white;
                Debug.Log("[0,0] GroupList size = " + hexGroups.Count);
            }
            
            hexGroups = gameTable[2][3].GetComponent<Hex>().GetAllPossibleHexGroups();
            group = hexGroups.First();

            {
                gameTable[2][3].GetComponent<SpriteRenderer>().color = Color.black;
                gameTable[group.firstHex.x][group.firstHex.y].GetComponent<SpriteRenderer>().color = Color.white;
                gameTable[group.secondHex.x][group.secondHex.y].GetComponent<SpriteRenderer>().color = Color.white;
                Debug.Log("[2,3] GroupList size = " + hexGroups.Count);
            }
            
            hexGroups = gameTable[7][2].GetComponent<Hex>().GetAllPossibleHexGroups();
            group = hexGroups.First();
            {
                gameTable[7][2].GetComponent<SpriteRenderer>().color = Color.black;
                gameTable[group.firstHex.x][group.firstHex.y].GetComponent<SpriteRenderer>().color = Color.white;
                gameTable[group.secondHex.x][group.secondHex.y].GetComponent<SpriteRenderer>().color = Color.white;
                Debug.Log("[7,2] GroupList size = " + hexGroups.Count);
            }
            
            hexGroups = gameTable[6][8].GetComponent<Hex>().GetAllPossibleHexGroups();
            group = hexGroups.First();
            {
                gameTable[6][8].GetComponent<SpriteRenderer>().color = Color.black;
                gameTable[group.firstHex.x][group.firstHex.y].GetComponent<SpriteRenderer>().color = Color.white;
                gameTable[group.secondHex.x][group.secondHex.y].GetComponent<SpriteRenderer>().color = Color.white;
                Debug.Log("[6,8] GroupList size = " + hexGroups.Count);
            }
            
            hexGroups = gameTable[0][7].GetComponent<Hex>().GetAllPossibleHexGroups();
            group = hexGroups.First();
            {
                gameTable[0][7].GetComponent<SpriteRenderer>().color = Color.black;
                gameTable[group.firstHex.x][group.firstHex.y].GetComponent<SpriteRenderer>().color = Color.white;
                gameTable[group.secondHex.x][group.secondHex.y].GetComponent<SpriteRenderer>().color = Color.white;
                Debug.Log("[0,7] GroupList size = " + hexGroups.Count);
            }
        }
    }
}