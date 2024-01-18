using DG.Tweening;
using Managers;
using UnityEngine;

namespace UI.Screens
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BaseScreen : MonoBehaviour
    {
        private Tween currentTween;

        public virtual void OnEnable()
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            if (currentTween != null)
            {
                currentTween.Kill();
            }

            canvasGroup.interactable = true;
            currentTween = DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1, 0.3f);
        }

        public virtual void Prepare(object param) {}

        public virtual void OnClickCloseButton()
        {
            HideScreen();
        }

        public virtual void HideScreen()
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            if (currentTween != null)
            {
                currentTween.Kill();
            }

            canvasGroup.interactable = false;
            currentTween = DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, 0.3f).OnComplete(OnHideScreenComplete);
        }

        public virtual void OnHideScreenComplete()
        {
            gameObject.SetActive(false);
            EventManager.Instance.TriggerEvent(EventNames.ScreenClosed, GetType());
        }
    }
    
}