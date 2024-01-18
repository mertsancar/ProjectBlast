using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using UI.Screens;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public TMP_Text moveCountText;
    public TargetBubblesLayout targetBubblesLayout;

    public bool IsPlaying;
    public int moveCount;

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 600;
    }

    private void Start()
    {
        SetGameEvents();
        
        SetLevel();

        SetCamera();
    }

    private void SetGameEvents()
    {
        EventManager.Instance.AddListener(EventNames.GameStart, () => IsPlaying = true);
        EventManager.Instance.AddListener(EventNames.GameStop, () => IsPlaying = false);
        EventManager.Instance.AddListener(EventNames.UpdateMoveCount, UpdateMoveCount);
        EventManager.Instance.AddListener(EventNames.LevelSuccess, OnLevelSuccess);
        EventManager.Instance.AddListener(EventNames.LevelFail, OnLevelFail);
        EventManager.Instance.AddListener(EventNames.UpdateTargetCards, UpdateTargetCards);
        EventManager.Instance.AddListener(EventNames.UpdateMoveText, UpdateMoveText);
    }
    
    private void SetLevel()
    {
        EventManager.Instance.TriggerEvent(EventNames.PrepareGame);
    }
    
    private void SetCamera()
    {
        var top = Screen.height;
        var mid = Screen.height / 2f;

        var topWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(0, top, -10));
        var midWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(0, mid, -10));

        var distance = Vector3.Distance(midWorldPoint, topWorldPoint);
        
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + distance, -10);
    }
    
    private void UpdateMoveCount()
    {
        moveCount--;
        UpdateMoveText();

        if (moveCount <= 0)
        {
            EventManager.Instance.TriggerEvent(EventNames.LevelFail);
        }
    }

    private void UpdateMoveText()
    {
        moveCountText.text = "Move " + moveCount;
    }
    
    private void UpdateTargetCards()
    {
        targetBubblesLayout.UpdateTargetCards();
    }
    
    private void OnLevelSuccess()
    {
        EventManager.Instance.TriggerEvent(EventNames.GameStop);
        
        var currentLevelIndex = PersistenceManager.GetCurrentLevelIndex();
        currentLevelIndex++;
        PersistenceManager.SetCurrentLevelIndex(currentLevelIndex);
        
        EventManager.Instance.TriggerEvent(EventNames.ShowScreenRequested, typeof(LevelSuccessScreen), null);
    }
    
    private void OnLevelFail()
    {
        EventManager.Instance.TriggerEvent(EventNames.GameStop);
        
        //EventManager.Instance.TriggerEvent(EventNames.ShowScreenRequested, typeof(LevelFailScreen), null);
    }

    public void OnClickSettingsButton()
    {
        EventManager.Instance.TriggerEvent(EventNames.ShowScreenRequested, typeof(SettingsScreen), null);
    }
}
