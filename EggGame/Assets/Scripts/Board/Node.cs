using DG.Tweening;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] private bool _isUsed;
    [SerializeField] private float _width;
    [SerializeField] private float _height;
    private int level;
    private int x, y;
    private SpriteRenderer _spriteRenderer;
    private Egg _currentEgg; // Cache egg reference for performance
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
    public int GetOrderInLayer()
    {
        if (_spriteRenderer != null)
        {
            return _spriteRenderer.sortingOrder;
        }
        return 0;
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
    public int GetPosX()
    {
        return x;
    }
    public int GetPosY()
    {
        return y;
    }
    public void OnCallNode()
    {
        transform.DOMove(transform.position + Vector3.up * 0.05f, 0.1f).SetEase(Ease.OutCubic);
        _spriteRenderer.DOColor(Color.green, 0.1f).SetEase(Ease.OutCubic);
    }
    public void OnReleaseNode()
    {
        transform.DOMove(transform.position - Vector3.up * 0.05f, 0.1f).SetEase(Ease.OutCubic);
        _spriteRenderer.DOColor(Color.white, 0.1f).SetEase(Ease.OutCubic);
    }

    public void SetEgg(Egg egg)
    {
        _currentEgg = egg;
        egg.SetOrderInLayer(_spriteRenderer.sortingOrder + 100);
        if (egg != null)
        {
            SetLevel(egg.GetLevel());
        }
        else
        {
            SetLevel(0);
        }
    }

    public Egg GetEgg()
    {
        return _currentEgg;
    }

    public bool HasEgg()
    {
        return _currentEgg != null;
    }

    public void RemoveEgg()
    {
        _currentEgg = null;
        SetLevel(0);
    }
}
