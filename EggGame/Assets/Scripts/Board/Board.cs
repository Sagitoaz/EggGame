using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private int _width = 5;
    [SerializeField] private int _height = 5;
    [SerializeField] private Node _nodePrefab;
    [SerializeField] private Node[] _nodePrefabs;
    private SpriteRenderer _spriteRenderer;
    private Node[,] _grid;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        _grid = new Node[_width, _height];
        InitializeBoard();

        EventManager.onReorganizeBoard += ReOrganizeBoard;
    }
    private void OnDestroy()
    {
        EventManager.onReorganizeBoard -= ReOrganizeBoard;
    }
    private void InitializeBoard()
    {
        Node tmpNode = Instantiate(_nodePrefab, Vector3.zero, Quaternion.identity, this.transform);
        float nodeWidth = tmpNode.GetWidth();
        float nodeHeight = tmpNode.GetHeight() - 0.05f;
        Destroy(tmpNode.gameObject);
        int orderinLayer = -5;

        // Resize the board background
        _spriteRenderer.size = new Vector2(_width * nodeWidth + 0.15f, _height * nodeHeight + 0.15f);

        float offsetX = (_width - 1) * nodeWidth / 2f;
        float offsetY = (_height - 1) * nodeHeight / 2f;
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Vector3 position = new Vector3(x * nodeWidth - offsetX, y * nodeHeight - offsetY, 0);

                // Instantiate node
                Node node = Instantiate(_nodePrefabs[(x + y) % 2], position, Quaternion.identity, this.transform);
                node.InitPos(x, y);
                node.name = $"Node ({x}, {y})";
                node.SetUsed(true);
                _grid[x, y] = node;
                node.SetOrderInLayer(orderinLayer);
                orderinLayer--;

                // Get an egg from the pool
                Egg egg = EggPool.Instance.GetEgg();
                egg.name = $"Egg ({x}, {y})";
                egg.SetParent(node);
                node.SetLevel(egg.GetLevel());
            }
        }
    }
    public void ReOrganizeBoard()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 1; y < _height; y++)
            {
                Node currentNode = _grid[x, y];
                Egg egg = currentNode.GetEgg();
                if (egg == null)
                {
                    continue;
                }
                
                // Find the lowest empty spot
                int targetY = y - 1;
                while (targetY >= 0 && _grid[x, targetY].GetLevel() == 0 && _grid[x, targetY].GetEgg() == null)
                {
                    targetY--;
                }
                targetY++; // Move back to the first empty spot
                
                // If we found a lower spot, move the egg there
                if (targetY < y)
                {
                    Node targetNode = _grid[x, targetY];
                    
                    // Remove egg from current node
                    currentNode.RemoveEgg();
                    
                    // Move egg to target node
                    egg.SetParent(targetNode);
                }
            }
        }
        SpawnNewEgg();
    }
    public Node[,] GetGrid()
    {
        return _grid;
    }
    
    // Debug method to clean up orphaned eggs
    private void SpawnNewEgg()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Node node = _grid[x, y];
                // Only spawn new egg if node has no egg AND level is 0
                if (node.GetLevel() == 0 && node.GetEgg() == null)
                {
                    Egg egg = EggPool.Instance.GetEgg();
                    egg.name = $"Egg ({x}, {y})";
                    egg.SetParent(node);
                }
            }
        }
    }
}
