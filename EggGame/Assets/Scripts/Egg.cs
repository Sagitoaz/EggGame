using System;
using DG.Tweening;
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
        Vector3 parentPos = new Vector3(parent.position.x, parent.position.y - 0.2f, parent.position.z);
        transform.DOMove(parentPos, 0.2f);
        
        Node parentNode = parent.GetComponent<Node>();
        if (parentNode != null)
        {
            parentNode.SetEgg(this);
        }
    }

    public void MoveTo(Transform target, float duration, Action onComplete = null)
    {
        Vector3 targetPos = new Vector3(target.position.x, target.position.y - 0.2f, target.position.z);
        transform.DOMove(targetPos, duration).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void AttachTo(Transform parent)
    {
        transform.SetParent(parent, true);
        
        // Update node reference for performance  
        Node parentNode = parent.GetComponent<Node>();
        if (parentNode != null)
        {
            parentNode.SetEgg(this);
        }
    }
}
