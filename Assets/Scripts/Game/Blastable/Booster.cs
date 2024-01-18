using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class Booster : BaseBlastable
{
    [SerializeField] private List<GameObject> boosterIcons;
    
    private BoosterType _type;
    public BoosterType GetBoosterType() => _type;

    public void Init(BoosterType type)
    {
        _type = type;

        boosterIcons.ForEach(icon => icon.SetActive(false));
        boosterIcons[(int)type].SetActive(true);
    }
    
    public override void Tap()
    {
        int totalCount = 1;
        var blastables = DistributionManager.blastables;
        var blastedIndexes = new List<int>();
        var collectedIndexes = new List<int>();
        
        for (int i = 0; i < blastables.Count; i++)
        {
            var blastable = blastables[i];
            if (blastable.transform.position.y < transform.position.y + 0.75f && blastable.transform.position.y > transform.position.y - 0.75f)
            {

                if (blastable.blastableType == BlastableType.Bubble)
                {
                    totalCount++;
                    
                    var bubble = (Bubble)blastable;
                    var target = GameController.Instance.targetBubblesLayout.GetTargets().Find(target => target.GetColor() == bubble.GetColor() && !target.isCompleted);
                    if (target)
                    {
                        collectedIndexes.Add(bubble.GetInstanceID());
                    }
                    else
                    {
                        blastedIndexes.Add(bubble.GetInstanceID());
                    }
                }
            }
        }
        
        blastedIndexes.Add(GetInstanceID());
        
        EventManager.Instance.TriggerEvent(EventNames.UpdateMoveCount);

        EventManager.Instance.TriggerEvent(EventNames.BlastBubbles, blastedIndexes);
        
        EventManager.Instance.TriggerEvent(EventNames.CollectBubbles, collectedIndexes);
        
    }
}

public enum BoosterType
{
    Horizontal = 0
}
