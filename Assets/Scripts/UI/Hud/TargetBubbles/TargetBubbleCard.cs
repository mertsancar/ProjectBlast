using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using Managers;
using TMPro;
using UnityEngine;

public class TargetBubbleCard : MonoBehaviour
{
    public BubbleColor color;
    public int count;
    public bool isCompleted;
    public AudioSource audioSource;
    
    [SerializeField] private TMP_Text countText;
    [SerializeField] private ParticleSystem completeEffect;

    public void Init(int _count)
    {
        count = _count;
        countText.text = count.ToString();
    }

    public void UpdateCount(int _count)
    {
        Init(_count);
        EventManager.Instance.TriggerEvent(EventNames.PlaySound, audioSource, AudioTag.CollectBubble, 0f);
    }

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
