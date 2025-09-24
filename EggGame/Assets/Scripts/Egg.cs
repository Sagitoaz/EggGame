using UnityEngine;

public class Egg : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private int _level;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetImage(Sprite sprite)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.sprite = sprite;
        }
    }
    public void SetLevel(int level)
    {
        _level = level;
    }
    public int GetLevel()
    {
        return _level;
    }
    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
    }
}
