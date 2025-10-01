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
    }
    private void InitializeBoard()
    {
        Node tmpNode = Instantiate(_nodePrefab, Vector3.zero, Quaternion.identity, this.transform);
        float nodeWidth = tmpNode.GetWidth();
        float nodeHeight = tmpNode.GetHeight() - 0.05f;
        Destroy(tmpNode.gameObject);
        int orderinLayer = -1;

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
                egg.SetParent(node.transform);
                node.SetLevel(egg.GetLevel());
            }
        }
    }
    public void ReOrganizeBoard()
    {
        Debug.Log("Reorganizing Board...");
        for (int x = 0; x < _width; x++)
        {
            for (int y = 1; y < _height; y++)
            {
                int bot = y;
                Egg egg = _grid[x, y].GetComponentInChildren<Egg>();
                if (egg == null)
                {
                    Debug.LogWarning($"No egg found at position ({x}, {y})");
                    continue;
                }
                bot--;
                if (_grid[x, bot].GetLevel() == 0)
                {
                    while (bot >= 0 && _grid[x, bot].GetLevel() == 0)
                    {
                        Debug.Log(x + " " + bot);
                        _grid[x, bot + 1].SetLevel(0);
                        egg.SetParent(_grid[x, bot].transform);
                        _grid[x, bot].SetLevel(egg.GetLevel());
                        bot--;
                    }
                }
            }
        }
    }
    public Node[,] GetGrid()
    {
        return _grid;
    }
}
