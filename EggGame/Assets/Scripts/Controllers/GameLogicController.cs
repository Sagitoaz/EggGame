using UnityEngine;

public class GameLogicController : MonoBehaviour
{
    [SerializeField] private Board _board;
    private Node[,] _grid;
    private Node _selectedNode;
    private BfsResult _currentBfsResult;
    private bool _isMerging = false;
    private bool _hasActiveSelection = false;

    private void Start()
    {
        EventManager.onNodeClicked += HandleNodeClicked;
        EventManager.onNodeReleased += HandleNodeReleased;
        EventManager.onMergeCompleted += HandleMergeCompleted;
    }

    private void OnDestroy()
    {
        EventManager.onNodeClicked -= HandleNodeClicked;
        EventManager.onNodeReleased -= HandleNodeReleased;
        EventManager.onMergeCompleted -= HandleMergeCompleted;
    }

    private void Update()
    {
        if (_board != null)
        {
            _grid = _board.GetGrid();
        }
    }

    private void HandleNodeClicked(Node clickedNode)
    {
        // Ignore input while merging
        if (_isMerging) return;

        if (_selectedNode == null)
        {
            // First click on node
            SelectNode(clickedNode);
        }
        else if (clickedNode == _selectedNode)
        {
            // Click on same node
            if (_currentBfsResult != null && _currentBfsResult.nodes.Count > 0)
            {
                _isMerging = true;
                EventManager.onValidMergeFound?.Invoke(_selectedNode, _currentBfsResult);
            }
            else
            {
                EventManager.onNodeReleased?.Invoke();
            }
        }
        else
        {
            EventManager.onNodeReleased?.Invoke();
            ClearState();
        }
    }

    private void SelectNode(Node node)
    {
        // Prevent selecting same node multiple times
        if (_hasActiveSelection && _selectedNode == node) return;

        _selectedNode = node;
        _hasActiveSelection = true;

        // Visual feedback for selected node
        node.OnCallNode();

        // Find merge candidates
        _currentBfsResult = BFS.FindSameLevelWithDepth(_grid, node.GetGridPosition(), false);

        // Highlight merge candidates (only if there are any)
        if (_currentBfsResult != null && _currentBfsResult.nodes.Count > 0)
        {
            foreach (Vector2Int pos in _currentBfsResult.nodes)
            {
                if (_grid[pos.x, pos.y] != null)
                {
                    _grid[pos.x, pos.y].OnCallNode();
                }
            }
        }
    }

    private void HandleNodeReleased()
    {
        if (!_hasActiveSelection || _selectedNode == null) return;

        // Remove visual feedback from selected node
        _selectedNode.OnReleaseNode();

        // Remove visual feedback from merge candidates
        if (_currentBfsResult != null && _currentBfsResult.nodes.Count > 0)
        {
            foreach (var pos in _currentBfsResult.nodes)
            {
                if (_grid[pos.x, pos.y] != null)
                {
                    _grid[pos.x, pos.y].OnReleaseNode();
                }
            }
        }     

        ClearState();   
    }

    private void HandleMergeCompleted(Node baseNode)
    {        
        _isMerging = false;
        
        _selectedNode = null;
        _currentBfsResult = null;
        _hasActiveSelection = false;

        EventManager.onReorganizeBoard?.Invoke();
    }

    private void ClearState()
    {
        _selectedNode = null;
        _currentBfsResult = null;
        _hasActiveSelection = false;
    }

    public void SetBoard(Board board)
    {
        _board = board;
    }
    
    public Node[,] GetGrid()
    {
        return _grid;
    }
}