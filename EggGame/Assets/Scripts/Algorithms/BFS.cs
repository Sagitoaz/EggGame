using System.Collections.Generic;
using UnityEngine;

public static class BFS
{
    private static Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(0, 1),   // Up
        new Vector2Int(1, 0),   // Right
        new Vector2Int(0, -1),  // Down
        new Vector2Int(-1, 0)   // Left
    };
    public static List<Vector2Int> FindConnectedSameLevel(Node[,] grid, Vector2Int start, bool includeStart = false)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        if (grid == null) return result;

        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        // Guard: start must be inside bounds and not null
        if (start.x < 0 || start.x >= width || start.y < 0 || start.y >= height) return result;
        Node startNode = grid[start.x, start.y];
        if (startNode == null) return result;

        int targetLevel = startNode.GetLevel();

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(start);
        visited.Add(start);

        if (includeStart)
        {
            result.Add(start);
        }

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighbor = current + dir;
                if (!IsInBounds(neighbor, width, height) || visited.Contains(neighbor))
                    continue;

                Node neighborNode = grid[neighbor.x, neighbor.y];
                if (neighborNode != null && neighborNode.GetLevel() == targetLevel)
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                    result.Add(neighbor);
                    neighborNode.OnCallNode();
                }
            }
        }

        return result;
    }
    private static bool IsInBounds(Vector2Int pos, int width, int height)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }
}
