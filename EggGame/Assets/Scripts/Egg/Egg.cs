using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class Egg : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject _shadow;
    private Transform _shadowTransform;
    private SpriteRenderer _shadowRenderer;
    private int _level;
    private Transform _visual;
    private Vector3 _visualBaseScale, _visualBaseLocalPos;
    private Vector3 _shadowBaseScale, _shadowBaseLocalPos;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _visual = this.transform;
        _shadowTransform = _shadow.transform;
        if (_shadowTransform)
        {
            _shadowRenderer = _shadowTransform.GetComponent<SpriteRenderer>();
            _shadow.SetActive(false);
        }
    }
    private void OnEnable()
    {
        EggAnimRunner.OnAnimStarted += HandleAnimStarted;
        EggAnimRunner.OnAnimCompleted += HandleAnimCompleted;
    }
    private void OnDisable()
    {
        EggAnimRunner.OnAnimStarted -= HandleAnimStarted;
        EggAnimRunner.OnAnimCompleted -= HandleAnimCompleted;
        EggAnimParams ctx = new EggAnimParams
        {
            Root = transform,
            Visual = _visual,
            Shadow = _shadowTransform,
            ShadowSR = _shadowRenderer,
            TweenId = GetInstanceID()
        };
        EggAnimRunner.Kill(ctx);
    }
    private void HandleAnimStarted(EggAnimType type, EggAnimParams p)
    {
        if (p.Root != transform) return;
    }
    private void HandleAnimCompleted(EggAnimType type, EggAnimParams p)
    {
        if (p.Root != transform) return;
    }
    public void SetImage(Sprite sprite)
    {
        if (_level > 1 && _shadow != null)
        {
            _shadow.SetActive(true);
        }
        if (_spriteRenderer != null)
        {
            _spriteRenderer.sprite = sprite;
        }
    }
    public void SetLevel(int level)
    {
        _level = level;
        if (_level <= 1 && _shadow != null)
        {
            _shadow.SetActive(false);
        }
    }
    public int GetLevel()
    {
        return _level;
    }
    public void SetParentByTransform(Transform parent)
    {
        transform.SetParent(parent, true);
        _visualBaseScale = _visual.localScale;
        _visualBaseLocalPos = _visual.localPosition;

        if (_shadowTransform)
        {
            _shadowTransform.SetParent(parent, true);
            _shadowTransform.position = new Vector3(parent.position.x, parent.position.y - 0.2f, parent.position.z);
        }
    }
    public void SetParent(Node node)
    {
        DOTween.Kill(this);
        transform.DOKill();

        Transform parent = node == null ? this.transform.parent : node.transform;

        transform.SetParent(parent, true);
        _visualBaseScale = _visual.localScale;
        _visualBaseLocalPos = _visual.localPosition;

        // Update node reference immediately to avoid race conditions
        if (node != null) node.SetEgg(this);

        transform.position = new Vector3(parent.position.x, transform.position.y, parent.position.z);

        Vector3 parentPos = new Vector3(parent.position.x, parent.position.y - 0.2f, parent.position.z);

        if (_shadowTransform)
        {
            _shadowTransform.SetParent(parent, true);
            _shadowTransform.position = parentPos;
        }

        transform.DOMove(parentPos, 0.2f).SetId(this).OnComplete(() =>
        {

            Node node = parent.GetComponent<Node>();
            if (node != null) node.SetEgg(this);

            if (GetLevel() > 1 && isActiveAndEnabled)
                StartIdleBounceLoop(parentPos);
        });
    }

    private void StartIdleBounceLoop(Vector3 parentPos)
    {
        if (GetLevel() > 1 && isActiveAndEnabled)
        {
            // Tạo context
            EggAnimParams ctx = new EggAnimParams
            {
                Root = transform,
                Visual = _visual,
                Shadow = _shadowTransform,
                ShadowSR = _shadowRenderer,
                BaseScale = _visual.localScale,
                GroundPos = parentPos,
                Level = _level,
                TweenId = GetInstanceID()
            };

            EggAnimRunner.PlayIdleBounce(ctx);
        }
    }

    public void MoveTo(Transform target, float duration, Action onComplete = null)
    {
        Vector3 targetPos = new Vector3(target.position.x, target.position.y - 0.2f, target.position.z);

        var ctx = new EggAnimParams
        {
            Root = transform,
            Visual = _visual,
            Shadow = _shadowTransform,
            ShadowSR = _shadowRenderer,
            BaseScale = _visual.localScale,
            GroundPos = targetPos,
            Level = _level,
            TweenId = GetInstanceID()
        };
        EggAnimRunner.PlayMoveTo(ctx, targetPos, duration, onComplete);
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

    public void SetOrderInLayer(int order)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.sortingOrder = order;
        }
    }
}
