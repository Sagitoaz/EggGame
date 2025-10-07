using UnityEngine;

public class InputController : MonoBehaviour
{
    private Camera _camera;
    private bool _isInputEnabled = true;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
        EventManager.onMergeStarted += OnMergeStarted;
        EventManager.onMergeCompleted += OnMergeCompleted;
    }

    private void OnDestroy()
    {
        EventManager.onMergeStarted -= OnMergeStarted;
        EventManager.onMergeCompleted -= OnMergeCompleted;
    }
    private void Update()
    {
        if (!_isInputEnabled) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }
    private void HandleMouseClick()
    {
        Vector3 worldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
        worldPoint.z = 0f;
        
        EventManager.onMouseClick?.Invoke(worldPoint);
        
        Collider2D hit = Physics2D.OverlapPoint(worldPoint);
        if (hit == null)
        {
            EventManager.onNodeReleased?.Invoke();
            return;
        }
        if (hit != null)
        {
            Node node = hit.GetComponent<Node>();
            if (node.GetLevel() <= 0) return;
            if (node != null)
            {
                EventManager.onNodeClicked?.Invoke(node);
            }
        }
    }

    private void OnMergeStarted(Node node)
    {
        _isInputEnabled = false;
    }

    private void OnMergeCompleted(Node node)
    {
        _isInputEnabled = true;
    }
}