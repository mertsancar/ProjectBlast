using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Managers
{
    public class DistributionManager : MonoBehaviour
    {
        public static List<BaseBlastable> blastables = new List<BaseBlastable>();
        public static float CollisionCheckDistance = 0.6f;
        public static float GotoTargetDuration = 0.15f;
        
        public List<Bubble> bubblePrefabs;
        public List<Booster> boosterPrefabs;
        public Transform blastablesObject;
        
        [SerializeField] private float ballInstantiatingDelay = 0.02f;
        [SerializeField] private float ballInstantiatingForce = 150f;
        
        private void Start()
        {
            EventManager.Instance.AddListener(EventNames.PrepareGame, () => StartCoroutine(InstantiateBubbles(LevelManager.CurrentLevelInfo)));
            EventManager.Instance.AddListener(EventNames.InstantiateBubbles, (bubbleColor) => InstantiateBubble((BubbleColor)bubbleColor));
            EventManager.Instance.AddListener(EventNames.InstantiateBubblesAfterBlast, (bubbleCount) => StartCoroutine(InstantiateBubblesAfterBlast((int)bubbleCount)));
            EventManager.Instance.AddListener(EventNames.InstantiateBooster, (booster, position) => InstantiateBooster((BoosterType)booster, (Vector2)position));
            EventManager.Instance.AddListener(EventNames.TapBlastable, (blastable) => TapBlastable((BaseBlastable)blastable));
            EventManager.Instance.AddListener(EventNames.BlastBubbles, (bubbles) => BlastBubbles((List<int>)bubbles));
            EventManager.Instance.AddListener(EventNames.CollectBubbles, (bubbles, target) => CollectBubbles((List<int>)bubbles, (TargetBubbleCard) target));
            EventManager.Instance.AddListener(EventNames.DestroyBlastable, (blastable) => StartCoroutine(DestroyBlastable((BaseBlastable)blastable)));
            EventManager.Instance.AddListener(EventNames.ResetBlastables, DestroyAllBlastables);
        }
        
        private  IEnumerator InstantiateBubbles(LevelInfo levelInfo)
        {
            yield return new WaitForSeconds(0.3f);
            var colorIndex = -1;
            var currentTarget = 0f;
            var bubblePrefabsRandom = new List<Bubble>();
            var ballCount = levelInfo.ballCount;
            for (int i = 0; i < ballCount; i++)
            {
                if (currentTarget <= i)
                {
                    colorIndex++;
                    currentTarget += ballCount * levelInfo.colorPercentages[colorIndex].percentage * 0.01f;
                }
                bubblePrefabsRandom.Add(bubblePrefabs[(int)(levelInfo.colorPercentages[colorIndex].color)]);
            }
            
            var count = bubblePrefabsRandom.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = Random.Range(i, count);
                (bubblePrefabsRandom[i], bubblePrefabsRandom[r]) = (bubblePrefabsRandom[r], bubblePrefabsRandom[i]);
            }
            
            for (int i = 0; i < ballCount; i++)
            {
                InstantiateBubble(bubblePrefabsRandom[i].GetColor());
                yield return new WaitForSeconds(ballInstantiatingDelay);
            }

            EventManager.Instance.TriggerEvent(EventNames.PlaySound, AudioTag.BubblesFalling, 0f);
            EventManager.Instance.TriggerEvent(EventNames.GameStart);
        }
        
        private IEnumerator InstantiateBubblesAfterBlast(int bubbleCount)
        {
            var indexes = GetBubblesColorIndexes();
            for (int i = 0; i < bubbleCount; i++)
            {
                var index = indexes[Random.Range(0, indexes.Count)];
                InstantiateBubble(bubblePrefabs[index].GetColor());
                yield return new WaitForSeconds(ballInstantiatingDelay);
            }
            
        }

        private List<int> GetBubblesColorIndexes()
        {
            var indexes = new List<int>();
            foreach (var blastable in blastables)
            {
                if (blastable.blastableType == BlastableType.Bubble)
                {
                    var bubble = (Bubble)blastable;
                    var index = (int)bubble.GetColor();
                    if (!indexes.Contains(index))
                    {
                        indexes.Add(index);                        
                    }
                }
            }

            return indexes;
        }
        
        private void InstantiateBubble(BubbleColor bubbleColor)
        {
            var bubbleObject = BubbleFactory(bubbleColor).transform;
            bubbleObject.position = new Vector3(Random.Range(-1f, 1f), Camera.main.transform.position.y + 12, 0);
            bubbleObject.rotation = Quaternion.identity;
            bubbleObject.SetParent(blastablesObject);
            bubbleObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -ballInstantiatingForce));

            var bubble = bubbleObject.GetComponent<Bubble>();
            blastables.Add(bubble);
            
        }
        
        private Bubble BubbleFactory(BubbleColor color)
        {
            return Instantiate(bubblePrefabs.Find(bubble => bubble.GetColor() == color), transform);
        }
        
        private void InstantiateBooster(BoosterType boosterType, Vector2 position)
        {
            var boosterObject = BoosterFactory(boosterType).transform;
            boosterObject.localPosition = position;
            boosterObject.rotation = Quaternion.identity;
            boosterObject.SetParent(blastablesObject);
            
            var booster = boosterObject.GetComponent<Booster>();
            blastables.Add(booster);
        }
    
        private Booster BoosterFactory(BoosterType type)
        {
            return Instantiate(boosterPrefabs.Find(booster => booster.GetBoosterType() == type), transform);
        }

        private void CollectBubbles(List<int> bubbleIndexes, TargetBubbleCard target)
        {
            for (int i = 0; i < bubbleIndexes.Count; i++)
            {
                var index = bubbleIndexes[i];
                var bubble = (Bubble)blastables.Find(bubble => bubble.GetInstanceID() == index);
                bubble.GotoTargetCard(target, i * GotoTargetDuration);
            }
        }
        
        private void TapBlastable(BaseBlastable blastable)
        {
            blastable.Tap();
        }
        
        private void BlastBubbles(List<int> bubbleIndexes)
        {
            foreach (var index in bubbleIndexes)
            {
                var bubble = blastables.Find(bubble => bubble.GetInstanceID() == index);
                bubble.Blast();
            }
            
            EventManager.Instance.TriggerEvent(EventNames.PlaySound, AudioTag.BlastBubbles, 0f);;
        }

        private IEnumerator DestroyBlastable(BaseBlastable blastable, float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            
            blastables.Remove(blastable);
            Destroy(blastable.gameObject);
            
            if (GameController.Instance.IsPlaying && GameController.Instance.targetBubblesLayout.CheckTargetsCompleted())
            {
                EventManager.Instance.TriggerEvent(EventNames.LevelSuccess);
            }
        }

        private void DestroyAllBlastables()
        {
            foreach (var blastable in blastables)
            {
                Destroy(blastable.gameObject);
            }
            blastables.Clear();
        }

        // private void FixedUpdate()
        // {
        //     foreach (var blastable in blastables)
        //     {
        //         if (blastable.blastableType == BlastableType.Booster)
        //         {
        //             return;
        //         }
        //         if (blastable.blastableType == BlastableType.Bubble)
        //         {
        //             var bubble = (Bubble)blastable;
        //             var sameColorBubbles = blastables.Where(blastable => blastable.blastableType == BlastableType.Bubble).Cast<Bubble>().ToList().FindAll(_bubble => _bubble.GetColor() == bubble.GetColor());
        //             var neighbours = GetNeighboursBubbles(bubble, new List<int>(), sameColorBubbles);
        //             
        //             if (neighbours.Count >= 3)
        //             {
        //                 return;
        //             }
        //             
        //             Debug.Log("SUHFFLE");
        //         }
        //     }
        // }
    }
    

}

