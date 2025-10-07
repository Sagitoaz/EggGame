using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EggPool : Singleton<EggPool>
{
    [SerializeField] private Egg _eggPrefab;
    [SerializeField] private List<Egg> _eggs = new List<Egg>();
    private EggData[] _soEggs;
    private int _poolSize = 30;
    protected override void Awake()
    {
        base.Awake();
        InitializePool();
    }
    private void InitializePool()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            Egg egg = Instantiate(_eggPrefab, this.transform);
            egg.gameObject.SetActive(false);
            _eggs.Add(egg);
        }
        _soEggs = Resources.LoadAll<EggData>(GameConfig.EGG_SO_PATH);
    }
    public Egg GetEgg()
    {
        foreach (Egg egg in _eggs)
        {
            if (!egg.gameObject.activeInHierarchy)
            {
                egg.gameObject.SetActive(true);
                int eggData = Random.Range(0, 3);
                egg.SetLevel(_soEggs[eggData].Level);
                egg.SetImage(_soEggs[eggData].EggSprite);
                return egg;
            }
        }
        Egg newEgg = Instantiate(_eggPrefab, this.transform);
        _eggs.Add(newEgg);
        _poolSize++;
        newEgg.gameObject.SetActive(true);
        return newEgg;
    }
    public void ReturnEgg(Egg egg)
    {
        if (egg == null) return;
        
        // Stop all animations first
        DOTween.Kill(egg);
        egg.transform.DOKill();
        
        // Reset position and parent
        egg.gameObject.transform.position = this.transform.position;
        egg.SetParentByTransform(this.transform);
        
        // Deactivate the egg
        egg.gameObject.SetActive(false);
    }
    public List<EggData> GetAllEggData()
    {
        return new List<EggData>(_soEggs);
    }
}
