
using DG.Tweening;
using Managers;
using UnityEngine;

namespace UI.Screens
{
    public class LevelFailScreen : BaseScreen
    {
        [SerializeField] private Transform levelFailText;
        [SerializeField] private Transform againButton;
        
        public override void Prepare(object param)
        {
            EventManager.Instance.TriggerEvent(EventNames.PlaySound, AudioTag.LevelFail, 0f);
            
            VibrationController.LevelFailVibration();
            
            levelFailText.gameObject.SetActive(false);
            againButton.gameObject.SetActive(false);

            var seq = DOTween.Sequence();
            seq.AppendInterval(0.5f);
            seq.AppendCallback(() => levelFailText.gameObject.SetActive(true));
            seq.AppendInterval(0.5f);
            seq.AppendCallback(() => againButton.gameObject.SetActive(true));
        }
        
        public void OnClickAgainButton()
        {
            HideScreen();
            
            EventManager.Instance.TriggerEvent(EventNames.ResetLevel);
        }
    }
    
}
