using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TargetBubbleCard : MonoBehaviour
{
    public bool isCompleted;
    
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private ParticleSystem completeEffect;
    [SerializeField] private AudioSource audioSource;
    
    private int _count;
    private BubbleColor _color;

    public void Init(int count, BubbleColor color)
    {
        _count = count;
        _color = color;
        countText.text = _count.ToString();
        image.color = this._color switch
        {
            BubbleColor.Red => Color.red,
            BubbleColor.Green => Color.green,
            BubbleColor.Blue => Color.blue,
            BubbleColor.Pink => Color.magenta,
            BubbleColor.Yellow => Color.yellow,
            _ => throw new ArgumentOutOfRangeException(nameof(this._color), this._color, null)
        };
    }

    public void UpdateCount(int _count)
    {
        Init(_count, _color);
        EventManager.Instance.TriggerEvent(EventNames.PlaySound, audioSource, AudioTag.CollectBubble, 0f);
    }
    
    public BubbleColor GetColor() => _color;
    
    public int GetCount() => _count;

    public void CompleteTarget()
    {
        if (!isCompleted)
        {
            countText.text = "Done";
            completeEffect.Play();
            isCompleted = true;
        }
    }
    
}
