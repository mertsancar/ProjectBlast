using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class Booster : BaseBlastable
{
    [SerializeField] private BoosterType type;
    public BoosterType GetBoosterType() => type;
    
    public override void Tap()
    {
        int totalCount = 1;
        var blastables = DistributionManager.blastables;
        var gotoTargetCounter = 1f;
        for (int i = 0; i < blastables.Count; i++)
        {
            var blastable = blastables[i];
            if (blastable.transform.position.y < transform.position.y + 0.75f && blastable.transform.position.y > transform.position.y - 0.75f)
            {
                totalCount++;

                if (blastable.blastableType == BlastableType.Bubble)
                {
                    var bubble = (Bubble)blastable;
                    var target = GameController.Instance.targetBubblesLayout.GetTargets().Find(target => target.color == bubble.GetColor() && !target.isCompleted);
                    if (target)
                    {
                        bubble.GotoTargetCard(target, gotoTargetCounter * DistributionManager.GotoTargetDuration);
                        gotoTargetCounter++;
                    }
                    else
                    {
                        bubble.Blast();  
                    }
                }
                // if (blastables[i].blastableType == BlastableType.Booster)
                // {
                //     var booster = (Booster)blastables[i];
                //     booster.Tap();
                // }

            }
        }
            
        Blast();

        EventManager.Instance.TriggerEvent(EventNames.InstantiateBubblesAfterBlast, totalCount);

    }
}

public enum BoosterType
{
    Horizontal = 0
}
