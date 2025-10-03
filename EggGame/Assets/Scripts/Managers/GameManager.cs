// GameManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Board _board;
    private Node[,] _grid;
    private Camera _camera;
    private int _lastX = -1, _lastY = -1;
    private BfsResult _bfsRes;
    private bool _isMerging = false;

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
        if (_isMerging) return;

        Vector3 point = _camera.ScreenToWorldPoint(Input.mousePosition);
        point.z = 0f;
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

            node.OnCallNode();
            _bfsRes = BFS.FindSameLevelWithDepth(_grid, node.GetGridPosition(), includeStart: false);
        }
        else if (node.GetPosX() == _lastX && node.GetPosY() == _lastY && _bfsRes != null && _bfsRes.nodes.Count > 0)
        {
            StartCoroutine(MergeThenUpgrade(node));
        }
        else
        {
            _grid[_lastX, _lastY].OnReleaseNode();
            if (_bfsRes != null)
            {
                foreach (var pos in _bfsRes.nodes) _grid[pos.x, pos.y].OnReleaseNode();
            }
            _bfsRes = null;
            _lastX = -1; _lastY = -1;
        }
    }

    private IEnumerator MergeThenUpgrade(Node baseNode)
    {
        _isMerging = true;
        Dictionary<int, List<Vector2Int>> byDepth = new Dictionary<int, List<Vector2Int>>();
        foreach (var p in _bfsRes.nodes)
        {
            int d = _bfsRes.depth[p];
            if (!byDepth.ContainsKey(d)) byDepth[d] = new List<Vector2Int>();
            byDepth[d].Add(p);
        }

        for (int d = _bfsRes.maxDepth; d >= 1; d--)
        {
            if (!byDepth.ContainsKey(d)) continue;
            List<Coroutine> batch = new List<Coroutine>();

            foreach (var pos in byDepth[d])
            {
                Node srcNode = _grid[pos.x, pos.y];
                if (srcNode == null) continue;

                Egg srcEgg = srcNode.GetComponentInChildren<Egg>();
                if (srcEgg == null) continue;

                if (!_bfsRes.parent.TryGetValue(pos, out var parentPos)) continue;
                Node dstNode = _grid[parentPos.x, parentPos.y];
                if (dstNode == null) continue;

                batch.Add(StartCoroutine(MoveAndPool(srcEgg, srcNode, dstNode)));
            }

            // wait merge
            foreach (var c in batch) yield return c;

            // delay to see the effect
            yield return new WaitForSeconds(0.05f);
        }

        Egg baseEgg = baseNode.GetComponentInChildren<Egg>();
        if (baseEgg != null)
        {
            baseEgg.SetLevel(baseEgg.GetLevel() + 1);
            baseNode.SetLevel(baseEgg.GetLevel());
            foreach (EggData data in EggPool.Instance.GetAllEggData())
            {
                if (data.Level == baseEgg.GetLevel())
                {
                    baseEgg.SetImage(data.EggSprite);
                    break;
                }
            }
        }
        baseNode.OnReleaseNode();

        _board.ReOrganizeBoard();

        // reset click
        _bfsRes = null;
        _lastX = -1; _lastY = -1;
        _isMerging = false;
    }

    //Move riêng xong bắn sự kiện để báo là dừng
    private IEnumerator MoveAndPool(Egg srcEgg, Node srcNode, Node dstNode)
    {
        bool done = false;
        srcEgg.MoveTo(dstNode.transform, 0.15f, () =>
        {
            EggPool.Instance.ReturnEgg(srcEgg);
            srcNode.SetLevel(0);
            srcNode.OnReleaseNode();
            done = true;
        });
        while (!done) yield return null;
    }
}
