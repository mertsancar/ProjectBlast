using System.Collections;
using System.Collections.Generic;
using Managers;
using UI.Screens;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : BaseScreen
{
    [SerializeField] private GameObject soundButtonCross;
    [SerializeField] private GameObject vibrationButtonCross;
    
    private bool isSoundActive;
    private bool isVibrationActive;
    
    public override void Prepare(object param)
    {
        EventManager.Instance.TriggerEvent(EventNames.GameStop);
        
        isSoundActive = AudioManager.Instance.ActiveSelf();
        isVibrationActive = VibrationController.ActiveSelf();

        soundButtonCross.SetActive(!isSoundActive);
        vibrationButtonCross.SetActive(!isVibrationActive);
    }

    public void OnClickSoundButton()
    {
        isSoundActive = AudioManager.Instance.ActiveSelf();
        soundButtonCross.SetActive(isSoundActive);
        AudioManager.Instance.SetActive(!isSoundActive);
        
    }
    
    public void OnClickVibrationButton()
    {
        isVibrationActive = VibrationController.ActiveSelf();
        vibrationButtonCross.SetActive(isVibrationActive);
        VibrationController.SetActive(!isVibrationActive);
    }
    
    public void OnClickCloseButton()
    {
        HideScreen();
        
        EventManager.Instance.TriggerEvent(EventNames.GameStart);
    }
    
}
