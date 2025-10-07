using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MergeController : MonoBehaviour
{
    [SerializeField] private GameLogicController _gameLogicController;
    private void Start()
    {
        EventManager.onValidMergeFound += HandleValidMergeFound;
    }

    private void OnDestroy()
    {
        EventManager.onValidMergeFound -= HandleValidMergeFound;
    }

    private void HandleValidMergeFound(Node baseNode, BfsResult bfsResult)
    {
        StartCoroutine(HandleMerge(baseNode, bfsResult));
    }

    private IEnumerator HandleMerge(Node baseNode, BfsResult bfsResult)
    {
        // Start merge node
        EventManager.onMergeStarted?.Invoke(baseNode);

        // Group nodes by depth
        Dictionary<int, List<Vector2Int>> nodesByDepth = GroupNodesByDepth(bfsResult);
        Node[,] grid = _gameLogicController.GetGrid();

        // Merge nodes from deepest
        for (int depth = bfsResult.maxDepth; depth >= 1; depth--)
        {
            if (!nodesByDepth.ContainsKey(depth)) continue;

            List<Coroutine> moveBatch = new List<Coroutine>();

            foreach (Vector2Int nodePos in nodesByDepth[depth])
            {
                Node sourceNode = grid[nodePos.x, nodePos.y];
                if (sourceNode == null) continue;

                // Use optimized egg reference instead of GetComponentInChildren
                Egg sourceEgg = sourceNode.GetEgg();
                if (sourceEgg == null) continue;

                // Find parent node to merge into
                if (!bfsResult.parent.TryGetValue(nodePos, out var parentPos)) continue;
                Node targetNode = grid[parentPos.x, parentPos.y];
                if (targetNode == null) continue;

                // Start moving egg
                moveBatch.Add(StartCoroutine(MoveEggToTarget(sourceEgg, sourceNode, targetNode)));
            }

            // Wait for eggs move
            foreach (Coroutine c in moveBatch)
            {
                yield return c;
            }

            // Delay between depth levels
            yield return new WaitForSeconds(0.05f);
        }

        // Upgrade the base egg
        yield return StartCoroutine(UpgradeBaseEgg(baseNode));

        // Complete merge
        EventManager.onMergeCompleted?.Invoke(baseNode);
    }

    private Dictionary<int, List<Vector2Int>> GroupNodesByDepth(BfsResult bfsResult)
    {
        Dictionary<int, List<Vector2Int>> nodesByDepth = new Dictionary<int, List<Vector2Int>>();
        
        foreach (Vector2Int nodePos in bfsResult.nodes)
        {
            int depth = bfsResult.depth[nodePos];
            if (!nodesByDepth.ContainsKey(depth))
            {
                nodesByDepth[depth] = new List<Vector2Int>();
            }
            nodesByDepth[depth].Add(nodePos);
        }
        
        return nodesByDepth;
    }

    private IEnumerator MoveEggToTarget(Egg sourceEgg, Node sourceNode, Node targetNode)
    {
        bool moveCompleted = false;
        
        // Clean up source node immediately
        sourceNode.RemoveEgg();
        sourceNode.OnReleaseNode();
        
        sourceEgg.MoveTo(targetNode.transform, 0.15f, () =>
        {
            // Stop all animations on the egg before returning to pool
            DOTween.Kill(sourceEgg);
            sourceEgg.transform.DOKill();
            
            // Return egg to pool immediately after animation
            EggPool.Instance.ReturnEgg(sourceEgg);
            moveCompleted = true;
        });

        // Wait for move to complete
        while (!moveCompleted)
        {
            yield return null;
        }
    }

    private IEnumerator UpgradeBaseEgg(Node baseNode)
    {
        // Use optimized egg reference instead of GetComponentInChildren
        Egg baseEgg = baseNode.GetEgg();
        if (baseEgg != null)
        {
            // Upgrade egg
            int newLevel = baseEgg.GetLevel() + 1;
            baseEgg.SetLevel(newLevel);
            baseNode.SetLevel(newLevel);

            // Update egg sprite
            foreach (EggData data in EggPool.Instance.GetAllEggData())
            {
                if (data.Level == newLevel)
                {
                    baseEgg.SetImage(data.EggSprite);
                    break;
                }
            }
        }

        baseNode.OnReleaseNode();
        
        yield return null;
    }

}