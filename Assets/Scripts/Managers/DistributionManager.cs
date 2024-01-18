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
        
        [SerializeField] private BlastablePooling blastablePooling;
        [SerializeField] private Transform blastablesObject;
        [SerializeField] private float bubbleInstantiatingDelay = 0.02f;
        [SerializeField] private float bubbleInstantiatingForce = 150f;
        
        private float _gotoTargetDuration = 0.1f;

        public int poolSize = 0;
        public int currentBubbleSize = 0;
        
        private void Awake()
        {
            blastables = new List<BaseBlastable>();
        }

        private void Update()
        {
            poolSize = blastablePooling.transform.childCount;
            currentBubbleSize = blastablesObject.transform.childCount;
        }

        private void Start()
        {
            EventManager.Instance.AddListener(EventNames.PrepareGame, () => StartCoroutine(InstantiateBubbles(LevelManager.CurrentLevelInfo)));
            EventManager.Instance.AddListener(EventNames.InstantiateBubbles, (bubbleColor) => InstantiateBubble((BubbleColor)bubbleColor));
            EventManager.Instance.AddListener(EventNames.InstantiateBubblesAfterBlast, (bubbleCount, delay) => StartCoroutine(InstantiateBubblesAfterBlast((int)bubbleCount, (float) delay)));
            EventManager.Instance.AddListener(EventNames.InstantiateBooster, (booster, position) => InstantiateBooster((BoosterType)booster, (Vector2)position));
            EventManager.Instance.AddListener(EventNames.TapBlastable, (blastable) => TapBlastable((BaseBlastable)blastable));
            EventManager.Instance.AddListener(EventNames.BlastBubbles, (bubbles) => BlastBubbles((List<int>)bubbles));
            EventManager.Instance.AddListener(EventNames.CollectBubbles, (bubbles) => CollectBubbles((List<int>)bubbles));
            EventManager.Instance.AddListener(EventNames.DestroyBlastable, (blastable) => StartCoroutine(DestroyBlastable((BaseBlastable)blastable)));
            EventManager.Instance.AddListener(EventNames.ResetBlastables, ResetAllBlastables);
        }
        
        private  IEnumerator InstantiateBubbles(LevelInfo levelInfo)
        {
            yield return new WaitForSeconds(0.3f);
            
            var colorIndex = -1;
            var currentTarget = 0f;
            var bubbleColorIndexesRandom = new List<int>();
            var ballCount = levelInfo.ballCount;
            for (int i = 0; i < ballCount; i++)
            {
                if (currentTarget <= i)
                {
                    colorIndex++;
                    currentTarget += ballCount * levelInfo.colorPercentages[colorIndex].percentage * 0.01f;
                }
                bubbleColorIndexesRandom.Add((int)levelInfo.colorPercentages[colorIndex].color);
            }
            
            var count = bubbleColorIndexesRandom.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = Random.Range(i, count);
                (bubbleColorIndexesRandom[i], bubbleColorIndexesRandom[r]) = (bubbleColorIndexesRandom[r], bubbleColorIndexesRandom[i]);
            }
            
            var bubbleColorList = Enum.GetValues(typeof(BubbleColor)).Cast<BubbleColor>().ToList();
            for (int i = 0; i < ballCount; i++)
            {
                InstantiateBubble(bubbleColorList[bubbleColorIndexesRandom[i]]);
                yield return new WaitForSeconds(bubbleInstantiatingDelay);
            }

            EventManager.Instance.TriggerEvent(EventNames.PlaySound, AudioTag.BubblesFalling, 0f);
            EventManager.Instance.TriggerEvent(EventNames.GameStart);
        }
        
        private IEnumerator InstantiateBubblesAfterBlast(int bubbleCount, float delay = 0f)
        {
            yield return new WaitForSeconds(delay);
            
            var indexes = GetBubblesColorIndexes();
            if (indexes.Count == 0) yield break;
            
            var bubbleColorList = Enum.GetValues(typeof(BubbleColor)).Cast<BubbleColor>().ToList();
            for (int i = 0; i < bubbleCount; i++)
            {
                var index = indexes[Random.Range(0, indexes.Count)];
                InstantiateBubble(bubbleColorList[index]);
                yield return new WaitForSeconds(bubbleInstantiatingDelay);
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
            var bubbleObject = blastablePooling.PopFromPool(bubbleColor).transform; 
            bubbleObject.position = new Vector3(Random.Range(-1f, 1f), Camera.main.transform.position.y + 12, 0);
            bubbleObject.rotation = Quaternion.identity;
            bubbleObject.SetParent(blastablesObject);
            bubbleObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -bubbleInstantiatingForce));

            var bubble = bubbleObject.GetComponent<Bubble>();
            blastables.Add(bubble);
            
        }
        
        private void InstantiateBooster(BoosterType boosterType, Vector2 position)
        {
            var boosterObject = blastablePooling.PopFromPool(boosterType).transform;
            boosterObject.localPosition = position;
            boosterObject.rotation = Quaternion.identity;
            boosterObject.SetParent(blastablesObject);
            
            var booster = boosterObject.GetComponent<Booster>();
            blastables.Add(booster);
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
            
            EventManager.Instance.TriggerEvent(EventNames.PlaySound, AudioTag.BlastBubbles, 0f);
            
            EventManager.Instance.TriggerEvent(EventNames.InstantiateBubblesAfterBlast, bubbleIndexes.Count, bubbleIndexes.Count * BaseBlastable.BlastEffectDuration);
        }
        
        private void CollectBubbles(List<int> bubbleIndexes)
        {
            for (int i = 0; i < bubbleIndexes.Count; i++)
            {
                var index = bubbleIndexes[i];
                var bubble = (Bubble)blastables.Find(b => b.GetInstanceID() == index);
                var target = GameController.Instance.targetBubblesLayout.GetTargets().Find(target => target.GetColor() == bubble.GetColor() && !target.isCompleted);
                bubble.GotoTargetCard(target, i * _gotoTargetDuration);
            }
            
            EventManager.Instance.TriggerEvent(EventNames.InstantiateBubblesAfterBlast, bubbleIndexes.Count, bubbleIndexes.Count * _gotoTargetDuration);
        }

        private IEnumerator DestroyBlastable(BaseBlastable blastable, float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            
            blastables.Remove(blastable);
            blastablePooling.PushToPool(blastable);
            
            if (GameController.Instance.IsPlaying && GameController.Instance.targetBubblesLayout.CheckTargetsCompleted())
            {
                EventManager.Instance.TriggerEvent(EventNames.LevelSuccess);
            }
        }

        private void ResetAllBlastables()
        {
            blastables.Clear();
        }

        // private void CheckGameLock()
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

