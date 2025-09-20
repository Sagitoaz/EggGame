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
    /// <summary>
    /// Returns all grid positions reachable from start that have the same Node level as the start node (4-directional adjacency).
    /// By default, excludes the start position from the result.
    /// </summary>
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
                }
            }
        }

        return result;
    }
    public static List<Vector2Int> BFSPathFinding(Node[,] grid, Vector2Int start)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> parentMap = new Dictionary<Vector2Int, Vector2Int>();
        List<Vector2Int> allNeighbors = new List<Vector2Int>();

        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        queue.Enqueue(start);
        visited.Add(start);
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighbor = current + dir;
                if (IsValidPosition(neighbor, visited, width, height))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                    parentMap[neighbor] = current;
                    allNeighbors.Add(neighbor);
                }
            }
        }
        return allNeighbors;
    }
    private static bool IsInBounds(Vector2Int pos, int width, int height)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }
    private static bool IsValidPosition(Vector2Int pos, HashSet<Vector2Int> visited, int width, int height)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height &&
               !visited.Contains(pos);
    }
}
