using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private int _width = 5;
    [SerializeField] private int _height = 5;
    [SerializeField] private Node _nodePrefab;
    [SerializeField] private Node[] _nodePrefabs;
    private Node[,] _grid;
    private void Start()
    {
        _grid = new Node[_width, _height];
        InitializeBoard();
    }
    private void InitializeBoard()
    {
        Node tmpNode = Instantiate(_nodePrefab, Vector3.zero, Quaternion.identity, this.transform);
        float nodeWidth = tmpNode.GetWidth() - 0.03f;
        float nodeHeight = tmpNode.GetHeight() - 0.1f;
        Destroy(tmpNode.gameObject);
        int orderinLayer = -1;

        float offsetX = (_width - 1) * nodeWidth / 2f;
        float offsetY = (_height - 1) * nodeHeight / 2f;
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Egg egg = EggPool.Instance.GetEgg();
                Vector3 position = new Vector3(x * nodeWidth - offsetX, y * nodeHeight - offsetY, 0);
                egg.transform.position = position;
                Node node = Instantiate(_nodePrefabs[(x + y) % 2], position, Quaternion.identity, this.transform);
                node.name = $"Node ({x}, {y})";
                _grid[x, y] = node;
                node.SetOrderInLayer(orderinLayer);
                orderinLayer--;
            }
        }
    }
}
