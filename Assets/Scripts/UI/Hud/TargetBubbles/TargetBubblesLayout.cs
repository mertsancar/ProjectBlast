using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class TargetBubblesLayout : MonoBehaviour
{
    [SerializeField] private List<TargetBubbleCard> cardPrefabs;
    [SerializeField] private List<TargetBubbleCard> cards;
    [SerializeField] private Transform layout;
    
    public List<TargetBubbleCard> GetTargets() => cards;
    
    public bool CheckTargetsCompleted()
    {
        foreach (var card in cards)
        {
            if (!card.isCompleted) return false;
        }

        return true;
    }

    public void UpdateTargetCards()
    {
        Init();
    }

    private void Init()
    {

        for (int i = 0; i < layout.transform.childCount; i++)
        {
            Destroy(layout.transform.GetChild(i).gameObject);
        }
        
        foreach (var card in cardPrefabs)
        {
            foreach (var targetColor in LevelManager.CurrentLevelInfo.targetColors)
            {
                if (targetColor.color == card.color)
                {
                    var bubbleCard = Instantiate(card, layout.transform).GetComponent<TargetBubbleCard>();
                    bubbleCard.Init(targetColor.count);
                    cards.Add(bubbleCard);
                }
            }
        }
    }
    
}
