using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;

public class BlastablePooling : MonoBehaviour
{
    public Bubble bubblePrefab;
    public Booster boosterPrefab;
    
    private Dictionary<BlastableType, List<BaseBlastable>> _blastablePool;

    public void PushToPool(BaseBlastable blastable)
    {
        switch (blastable.blastableType)
        {
            case BlastableType.Bubble:
                PushToBubblePool((Bubble)blastable);
                return;
            case BlastableType.Booster:
                PushToBoosterPool((Booster)blastable);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public Bubble PopFromPool(BubbleColor bubbleColor)
    {
        if (_blastablePool == null) Init();
        
        var pool = _blastablePool[BlastableType.Bubble];
        if (pool.Count == 0) PushToBubblePool(BubbleFactory(bubbleColor));
        var bubble = (Bubble)pool.Last();
        bubble.Init(bubbleColor);
        pool.Remove(bubble);
        bubble.gameObject.SetActive(true);
        return bubble;
    }
    
    public Booster PopFromPool(BoosterType type)
    {
        if (_blastablePool == null) Init();
        
        var pool = _blastablePool[BlastableType.Booster];
        if (pool.Count == 0) PushToBoosterPool(BoosterFactory(type));
        var booster = (Booster)pool.Last();
        booster.Init(type);
        pool.Remove(booster);
        booster.gameObject.SetActive(true);
        return booster;
    }

    private void PushToBubblePool(Bubble bubble)
    {
        if (_blastablePool == null) Init();
        
        var pool = _blastablePool[BlastableType.Bubble];
        pool.Add(bubble);
        bubble.transform.SetParent(transform);
        bubble.gameObject.SetActive(false);
    }
    
    private void PushToBoosterPool(Booster booster)
    {
        if (_blastablePool == null) Init();
        
        var pool = _blastablePool[BlastableType.Booster];
        pool.Add(booster);
        booster.transform.SetParent(transform);
        booster.gameObject.SetActive(false);
    }
    
    private void Init()
    {
        _blastablePool = new Dictionary<BlastableType, List<BaseBlastable>>();

        var blastableTypeList = Enum.GetValues(typeof(BlastableType)).Cast<BlastableType>().ToList();

        foreach (var blastableType in blastableTypeList)
        {
            _blastablePool[blastableType] = new List<BaseBlastable>();
        }
        
    }

    private Bubble BubbleFactory(BubbleColor color)
    {
        var bubble = Instantiate(bubblePrefab, transform);
        bubble.Init(color);
        return bubble;
    }
    
    private Booster BoosterFactory(BoosterType type)
    {
        var booster = Instantiate(boosterPrefab, transform);
        booster.Init(type);
        return booster;
    }
        
 
}
