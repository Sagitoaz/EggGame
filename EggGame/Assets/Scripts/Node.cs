using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] private bool _isUsed;
    [SerializeField] private float _width;
    [SerializeField] private float _height;
    private SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _width = _spriteRenderer.bounds.size.x;
        _height = _spriteRenderer.bounds.size.y;
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
}
