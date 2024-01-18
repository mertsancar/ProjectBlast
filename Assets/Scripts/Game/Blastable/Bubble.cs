using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Managers;
using UnityEngine;

public class Bubble : BaseBlastable
{
    [SerializeField] private BubbleColor color;
    public BubbleColor GetColor() => color;

    public override void Tap()
    {
        var sameColorBalls = DistributionManager.blastables
            .Where(blastable => blastable.blastableType == BlastableType.Bubble)
            .Cast<Bubble>()
            .ToList()
            .FindAll(bubble => bubble.GetColor() == GetColor());
        
        var bubblesIndexes = GetNeighboursBubbles(this, new List<int>(), sameColorBalls);
        var target = GameController.Instance.targetBubblesLayout.GetTargets().Find(target => target.color == GetColor() && !target.isCompleted);
        
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
                EventManager.Instance.TriggerEvent(EventNames.CollectBubbles, bubblesIndexes, target);
            }
            else
            {
                EventManager.Instance.TriggerEvent(EventNames.BlastBubbles, bubblesIndexes);
            }
            
            EventManager.Instance.TriggerEvent(EventNames.InstantiateBubblesAfterBlast, bubblesIndexes.Count);
        }
    }

    public void GotoTargetCard(TargetBubbleCard target, float delay = 0)
    {
        collider.enabled = false;
        transform.DOMove(target.transform.position, delay).OnComplete(() =>
        {
            var count = target.count;
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
            if (distance < DistributionManager.CollisionCheckDistance && !neighbourList.Contains(bubble.GetInstanceID()))
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
    Orange = 4
}

