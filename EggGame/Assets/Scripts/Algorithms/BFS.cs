// BFS.cs
using System.Collections.Generic;
using UnityEngine;

public class BfsResult
{
    public Dictionary<Vector2Int, int> depth = new Dictionary<Vector2Int, int>();
    public Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();
    public List<Vector2Int> nodes = new List<Vector2Int>();
    public int maxDepth = 0;
}

public static class BFS
{
    private static Vector2Int[] directions = new Vector2Int[] {
        new Vector2Int(0, 1), new Vector2Int(1, 0),
        new Vector2Int(0, -1), new Vector2Int(-1, 0)
    };

    public static BfsResult FindSameLevelWithDepth(Node[,] grid, Vector2Int start, bool includeStart = false)
    {
        var res = new BfsResult();
        if (grid == null) return res;
        int w = grid.GetLength(0), h = grid.GetLength(1);
        if (start.x < 0 || start.x >= w || start.y < 0 || start.y >= h) return res;
        Node startNode = grid[start.x, start.y];
        if (startNode == null) return res;

        int targetLv = startNode.GetLevel();
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        q.Enqueue(start);
        visited.Add(start);
        res.depth[start] = 0;
        if (includeStart) res.nodes.Add(start);

        while (q.Count > 0)
        {
            var cur = q.Dequeue();
            foreach (var d in directions)
            {
                var nb = cur + d;
                if (nb.x < 0 || nb.x >= w || nb.y < 0 || nb.y >= h) continue;
                if (visited.Contains(nb)) continue;

                Node nbNode = grid[nb.x, nb.y];
                if (nbNode != null && nbNode.GetLevel() == targetLv)
                {
                    visited.Add(nb);
                    q.Enqueue(nb);
                    res.parent[nb] = cur;
                    int dep = res.depth[cur] + 1;
                    res.depth[nb] = dep;
                    res.maxDepth = Mathf.Max(res.maxDepth, dep);
                    res.nodes.Add(nb);
                    nbNode.OnCallNode();
                }
            }
        }
        return res;
    }
}
