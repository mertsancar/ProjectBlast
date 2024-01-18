using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance;
            
        private Dictionary<string, Action> _eventDictionary0;
        private Dictionary<string, Action<object>> _eventDictionary1;
        private Dictionary<string, Action<object, object>> _eventDictionary2;
        private Dictionary<string, Action<object, object, object>> _eventDictionary3;

        private void Awake()
        {
            Instance = this;
            SceneManager.sceneUnloaded += ClearListeners;
            SceneManager.sceneUnloaded += ClearAllTween;
        }

        public void AddListener(string eventName, Action listener)
        {
            if (_eventDictionary0 == null) _eventDictionary0 = new Dictionary<string, Action>();
            if (_eventDictionary0.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent += listener;
                _eventDictionary0[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                _eventDictionary0.Add(eventName, thisEvent);
            }
        }
        
        public void AddListener(string eventName, Action<object> listener)
        {
            if (_eventDictionary1 == null) _eventDictionary1 = new Dictionary<string, Action<object>>();
            if (_eventDictionary1.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent += listener;
                _eventDictionary1[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                _eventDictionary1.Add(eventName, thisEvent);
            }
        }

        private void ClearListeners(Scene scene)
        {
            _eventDictionary0?.Clear();
            _eventDictionary1?.Clear();
            _eventDictionary2?.Clear();
            _eventDictionary3?.Clear();
        }

        private void ClearAllTween(Scene scene)
        {
            if (scene.name == "Initial") return;
            DOTween.KillAll();
        }
            
        public void AddListener(string eventName, Action<object, object> listener)
        {
            if (_eventDictionary2 == null) _eventDictionary2 = new Dictionary<string, Action<object, object>>();
            if (_eventDictionary2.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent += listener;
                _eventDictionary2[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                _eventDictionary2.Add(eventName, thisEvent);
            }
        }
        
        public void AddListener(string eventName, Action<object, object, object> listener)
        {
            if (_eventDictionary3 == null) _eventDictionary3 = new Dictionary<string, Action<object, object, object>>();
            if (_eventDictionary3.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent += listener;
                _eventDictionary3[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                _eventDictionary3.Add(eventName, thisEvent);
            }
        }

        public void TriggerEvent(string eventName)
        {
            if (_eventDictionary0 == null)
            {
                Debug.LogWarning("[EventManager] TriggerEvent:: Event couldn't be triggered because there are no listeners");
                return;
            }

            if (_eventDictionary0.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent.Invoke();
            }
        }

        public void TriggerEvent(string eventName, object arg1)
        {
            if (_eventDictionary1 == null)
            {
                Debug.LogWarning("[EventManager] TriggerEvent:: Event couldn't be triggered because there are no listeners");
                return;
            }

            if (_eventDictionary1.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent.Invoke(arg1);
            }
        }
            
        public void TriggerEvent(string eventName, object arg1, object arg2)
        {
            if (_eventDictionary2 == null)
            {
                Debug.LogWarning("[EventManager] TriggerEvent:: Event couldn't be triggered because there are no listeners");
                return;
            }

            if (_eventDictionary2.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent.Invoke(arg1, arg2);
            }
        }
        
        public void TriggerEvent(string eventName, object arg1, object arg2, object arg3)
        {
            if (_eventDictionary3 == null)
            {
                Debug.LogWarning("[EventManager] TriggerEvent:: Event couldn't be triggered because there are no listeners");
                return;
            }

            if (_eventDictionary3.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent.Invoke(arg1, arg2, arg3);
            }
        }
    }
    
}



public static class EventNames
{
    public static readonly string ShowScreenRequested = "ShowScreenRequested";
    public static readonly string HideScreenRequested = "HideScreenRequested";
    public static readonly string ScreenClosed = "ScreenClosed";
    public static readonly string ScreenShown = "ScreenShown";
    public static readonly string PlaySound = "PlaySound";
    public static readonly string PrepareGame = "PrepareGame";
    public static readonly string SetCamera = "SetCamera";
    public static readonly string GameStart = "GameStart";
    public static readonly string GameStop = "GameStop";
    public static readonly string InstantiateBooster = "InstantiateBooster";
    public static readonly string InstantiateBubbles = "InstantiateBubbles";
    public static readonly string InstantiateBubblesAfterBlast = "InstantiateBubblesAfterBlast";
    public static readonly string TapBlastable = "TapBlastable";
    public static readonly string BlastBubbles = "BlastBubbles";
    public static readonly string CollectBubbles = "CollectBubbles";
    public static readonly string DestroyBlastable = "DestroyBlastable";
    public static readonly string UpdateMoveCount = "UpdateMoveCount";
    public static readonly string UpdateTargetCards = "UpdateTargetCards";
    public static readonly string LevelSuccess = "LevelSuccess";
    public static readonly string LevelFail = "LevelFail";
    public static readonly string ResetLevel = "ResetLevel";
    public static readonly string ResetBlastables = "ResetBlastables";

}