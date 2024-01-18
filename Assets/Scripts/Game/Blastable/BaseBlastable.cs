using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Managers;
using UnityEngine;

public abstract class BaseBlastable : MonoBehaviour
{
    public BlastableType blastableType;
    
    [SerializeField] protected ParticleSystem blastEffect;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected CircleCollider2D collider;
    
    private float BlastEffectDuration = 0.15f;

    public abstract void Tap();
    
    public void Blast()
    {
        var seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            blastEffect.Play();
        });
        seq.AppendInterval(BlastEffectDuration).OnComplete(() =>
        {
            EventManager.Instance.TriggerEvent(EventNames.DestroyBlastable, this);
        });
    }

}

public enum BlastableType
{
    Bubble,
    Booster
}