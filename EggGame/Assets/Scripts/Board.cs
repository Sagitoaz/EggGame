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
        for (int x = 0; x < _width; x++)
        {
            for (int y = 1; y < _height; y++)
            {
                int bot = y;
                Egg egg = _grid[x, y].GetEgg();
                if (egg == null)
                {
                    continue;
                }
                bot--;
                if (_grid[x, bot].GetLevel() == 0)
                {
                    while (bot >= 0 && _grid[x, bot].GetLevel() == 0)
                    {                        
                        _grid[x, bot + 1].RemoveEgg();
                        egg.SetParent(_grid[x, bot].transform);                        
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

    // Debug method to validate egg references
    [ContextMenu("Validate Egg References")]
    public void ValidateEggReferences()
    {
        int validReferences = 0;
        int invalidReferences = 0;
        
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Node node = _grid[x, y];
                Egg eggRef = node.GetEgg();
                Egg eggChild = node.GetComponentInChildren<Egg>();
                
                if (eggRef == eggChild)
                {
                    validReferences++;
                }
                else
                {
                    invalidReferences++;
                    Debug.LogWarning($"Egg reference mismatch at ({x}, {y}): ref={eggRef}, child={eggChild}");
                }
            }
        }
        
        Debug.Log($"Egg Reference Validation: {validReferences} valid, {invalidReferences} invalid");
    }
}
