using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;

public class BlastablePooling : MonoBehaviour
{
    public List<Bubble> bubblePrefabs;
    public List<Booster> boosterPrefabs;
    private Dictionary<BubbleColor, List<Bubble>> _bubblePool;
    private Dictionary<BoosterType, List<Booster>> _boosterPool;

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
        if (_bubblePool == null) Init();
        
        var pool = _bubblePool[bubbleColor];
        if (pool.Count == 0) PushToBubblePool(BubbleFactory(bubbleColor));
        var bubble = pool.Last();
        pool.Remove(bubble);
        bubble.gameObject.SetActive(true);
        return bubble;
    }
    
    public Booster PopFromPool(BoosterType type)
    {
        if (_bubblePool == null) Init();
        
        var pool = _boosterPool[type];
        if (pool.Count == 0) PushToBoosterPool(BoosterFactory(type));
        var bubble = pool.Last();
        pool.Remove(bubble);
        bubble.gameObject.SetActive(true);
        return bubble;
    }

    public List<int> GetBubblePoolColorsIndexes()
    {
        var bubblePoolList = _bubblePool.ToList(); 
        var indexes = new List<int>();
        for (int i = 0; i < _bubblePool.Count; i++)
        {
            var index = (int)bubblePoolList[i].Key;
            if (!indexes.Contains(index)) indexes.Add(index);
        }

        return indexes;
    }

    private void PushToBubblePool(Bubble bubble)
    {
        if (_bubblePool == null) Init();
        
        var pool = _bubblePool[bubble.GetColor()];
        pool.Add(bubble);
        bubble.transform.SetParent(transform);
        bubble.gameObject.SetActive(false);
    }
    
    private void PushToBoosterPool(Booster booster)
    {
        if (_bubblePool == null) Init();
        
        var pool = _boosterPool[booster.GetBoosterType()];
        pool.Add(booster);
        booster.transform.SetParent(transform);
        booster.gameObject.SetActive(false);
    }
    
    private void Init()
    {
        _bubblePool = new Dictionary<BubbleColor, List<Bubble>>();
        _boosterPool = new Dictionary<BoosterType, List<Booster>>();

        var bubbleColorList = Enum.GetValues(typeof(BubbleColor)).Cast<BubbleColor>().ToList();
        var boosterTypeList = Enum.GetValues(typeof(BoosterType)).Cast<BoosterType>().ToList();

        foreach (var bubbleColor in bubbleColorList)
        {
            _bubblePool[bubbleColor] = new List<Bubble>();
        }

        foreach (var boosterType in boosterTypeList)
        {
            _boosterPool[boosterType] = new List<Booster>();
        }
        
    }

    private Bubble BubbleFactory(BubbleColor color)
    {
        return Instantiate(bubblePrefabs.Find(bubble => bubble.GetColor() == color), transform);
    }
    
    private Booster BoosterFactory(BoosterType type)
    {
        return Instantiate(boosterPrefabs.Find(booster => booster.GetBoosterType() == type), transform);
    }
        
 
}
