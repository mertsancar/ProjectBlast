using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Managers;
using UnityEngine;

public class Bubble : BaseBlastable
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private BubbleColor _color;
    private static float _collisionCheckDistance = 0.6f;

    public void Init(BubbleColor color)
    {
        _color = color;
        spriteRenderer.color = color switch
        {
            BubbleColor.Red => Color.red,
            BubbleColor.Green => Color.green,
            BubbleColor.Blue => Color.blue,
            BubbleColor.Pink => Color.magenta,
            BubbleColor.Orange => Color.yellow,
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
        };

    }
    
    public BubbleColor GetColor() => _color;

    public override void Tap()
    {
        var sameColorBalls = DistributionManager.blastables
            .Where(blastable => blastable.blastableType == BlastableType.Bubble)
            .Cast<Bubble>()
            .ToList()
            .FindAll(bubble => bubble.GetColor() == GetColor());
        
        var bubblesIndexes = GetNeighboursBubbles(this, new List<int>(), sameColorBalls);
        var target = GameController.Instance.targetBubblesLayout.GetTargets().Find(target => target.GetColor() == GetColor() && !target.isCompleted);
        
        if (bubblesIndexes.Count >= 4 && !target)
        {
            EventManager.Instance.TriggerEvent(EventNames.UpdateMoveCount);
            EventManager.Instance.TriggerEvent(EventNames.InstantiateBooster, BoosterType.Horizontal, new Vector2(transform.localPosition.x, transform.localPosition.y));
            EventManager.Instance.TriggerEvent(EventNames.BlastBubbles, bubblesIndexes);
        }
        if (bubblesIndexes.Count >= 3)
        {
            EventManager.Instance.TriggerEvent(EventNames.UpdateMoveCount);
                
            if (target)
            {
                EventManager.Instance.TriggerEvent(EventNames.CollectBubbles, bubblesIndexes);
            }
            else
            {
                EventManager.Instance.TriggerEvent(EventNames.BlastBubbles, bubblesIndexes);
            }
            
        }
    }

    public void GotoTargetCard(TargetBubbleCard target, float delay = 0)
    {
        collider.enabled = false;
        transform.DOMove(target.transform.position, delay).OnComplete(() =>
        {
            var count = target.GetCount();
            count--;
            if (count == 0)
            {
                target.CompleteTarget();
            }
            else
            {
                target.UpdateCount(count);
            }

            collider.enabled = true;
            
            EventManager.Instance.TriggerEvent(EventNames.DestroyBlastable, this);
        });
    }
    
    private List<int> GetNeighboursBubbles(Bubble currentBall, List<int> neighbourList, List<Bubble> sameColorBalls)
    {
        var currentBallPos = currentBall.transform.position;
        foreach (var bubble in sameColorBalls)
        {
            var distance = Vector3.Distance(bubble.transform.position, currentBallPos);
            if (distance < _collisionCheckDistance && !neighbourList.Contains(bubble.GetInstanceID()))
            {
                neighbourList.Add(bubble.GetInstanceID());
                var ret = GetNeighboursBubbles(bubble, neighbourList, sameColorBalls);
                foreach (var index in ret)
                {
                    if (!neighbourList.Contains(index))
                    {
                        neighbourList.Add(index);
                    }
                }
            }
        }
            
        return neighbourList;
    }

}

public enum BubbleColor
{
    Red = 0,
    Green = 1,
    Blue = 2,
    Pink = 3,
    Orange = 4,
}

