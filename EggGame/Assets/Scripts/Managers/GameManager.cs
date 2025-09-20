using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Board _board;
    private Node[,] _grid;
    private Camera _camera;
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
                Debug.Log($"Clicked on Node: {node.name} | Level: {node.GetLevel()}");
                List<Vector2Int> sameLevel = BFS.FindConnectedSameLevel(_grid, node.GetGridPosition());
                if (sameLevel.Count == 0)
                {
                    Debug.Log("No adjacent same-level nodes found.");
                }
                else
                {
                    Debug.Log($"Found {sameLevel.Count} connected same-level node(s):");
                    foreach (Vector2Int pos in sameLevel)
                    {
                        Debug.Log($"  -> Node ({pos.x}, {pos.y})");
                    }
                }
            }
        }
    }
}