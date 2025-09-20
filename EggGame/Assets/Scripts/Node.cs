using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] private bool _isUsed;
    [SerializeField] private float _width;
    [SerializeField] private float _height;
    private int level;
    private int x, y;
    private SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _width = _spriteRenderer.bounds.size.x;
        _height = _spriteRenderer.bounds.size.y;
    }
    public void InitPos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public float GetWidth()
    {
        return _width;
    }
    public float GetHeight()
    {
        return _height;
    }
    public void SetOrderInLayer(int order)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.sortingOrder = order;
        }
    }
    public void SetUsed(bool isUsed)
    {
        _isUsed = isUsed;
    }
    public Vector2Int GetGridPosition()
    {
        return new Vector2Int(x, y);
    }
    public int GetLevel()
    {
        return level;
    }
    public void SetLevel(int level)
    {
        this.level = level;
    }
}
