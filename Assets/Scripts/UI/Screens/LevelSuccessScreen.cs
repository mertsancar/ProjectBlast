
using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Screens
{
    public class LevelSuccessScreen : BaseScreen
    {
        [SerializeField] private Transform levelSuccessText;
        [SerializeField] private Transform nextButton;
        
        public override void Prepare(object param)
        {
            EventManager.Instance.TriggerEvent(EventNames.PlaySound, AudioTag.LevelSuccess, 0f);
            
            VibrationController.LevelSuccessVibration();
            
            levelSuccessText.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);

            var seq = DOTween.Sequence();
            seq.AppendInterval(0.5f);
            seq.AppendCallback(() => levelSuccessText.gameObject.SetActive(true));
            seq.AppendInterval(0.5f);
            seq.AppendCallback(() => nextButton.gameObject.SetActive(true));
        }

        public void OnClickNextButton()
        {
            SceneManager.LoadScene("Game");
        }
    }
}
