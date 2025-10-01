using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Board _board;
    private Node[,] _grid;
    private Camera _camera;
    private int _lastX = -1, _lastY = -1;
    private List<Vector2Int> _nodeSameLv;
    protected override void Awake()
    {
        base.Awake();
        _camera = Camera.main;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnPointerDown();
        }
        _grid = _board.GetGrid();
    }
    private void OnPointerDown()
    {
        Vector3 point = _camera.ScreenToWorldPoint(Input.mousePosition);
        point.z = 0f;
        Vector3 origin = _camera.transform.position;
        Vector3 dir = (point - origin).normalized * 10f;

        Debug.DrawRay(origin, dir, Color.red, 3f);

        Collider2D hit = Physics2D.OverlapPoint(point);
        if (hit != null)
        {
            Node node = hit.GetComponent<Node>();
            if (node != null)
            {
                HandleClickedNode(node);
            }
        }
    }
    private void HandleClickedNode(Node node)
    {
        if (_lastX == -1 && _lastY == -1)
        {
            _lastX = node.GetPosX();
            _lastY = node.GetPosY();

            Debug.Log($"Clicked on Node at {node.GetGridPosition()} with level {node.GetLevel()}");
            node.OnCallNode();

            //BFS search
            _nodeSameLv = BFS.FindConnectedSameLevel(_grid, node.GetGridPosition());
        }
        else if (node.GetPosX() == _lastX && node.GetPosY() == _lastY && _nodeSameLv != null && _nodeSameLv.Count > 0)
        {
            foreach (Vector2Int pos in _nodeSameLv)
            {
                Node n = _grid[pos.x, pos.y];
                if (n != null)
                {
                    n.OnReleaseNode();
                    Egg e = n.GetComponentInChildren<Egg>();
                    if (e != null)
                    {
                        EggPool.Instance.ReturnEgg(e);
                        n.SetLevel(0);
                    }
                }
            }
            _nodeSameLv.Clear();
            _lastX = -1;
            _lastY = -1;

            //Increase level of clicked node
            Egg egg = node.GetComponentInChildren<Egg>();
            if (egg != null)
            {
                egg.SetLevel(egg.GetLevel() + 1);
                node.SetLevel(egg.GetLevel());
                foreach (EggData data in EggPool.Instance.GetAllEggData())
                {
                    if (data.Level == egg.GetLevel())
                    {
                        egg.SetImage(data.EggSprite);
                        break;
                    }
                }
            }
            node.OnReleaseNode();
            _board.ReOrganizeBoard();
        }
        else
        {
            _grid[_lastX, _lastY].OnReleaseNode();
            foreach (Vector2Int pos in _nodeSameLv)
            {
                _grid[pos.x, pos.y].OnReleaseNode();
            }
            _nodeSameLv.Clear();
            _lastX = -1;
            _lastY = -1;
        }
    }
}