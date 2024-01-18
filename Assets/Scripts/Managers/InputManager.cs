using Managers;
using UI.Screens;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && GameController.Instance.IsPlaying)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            
            if(hit.collider != null)
            {
                var hitObject = hit.collider.gameObject;

                if (hitObject.CompareTag("Blastable"))
                {
                    VibrationController.TapBubbleVibration();
                    var blastable = hitObject.GetComponent<BaseBlastable>();
                    EventManager.Instance.TriggerEvent(EventNames.TapBlastable, blastable);
                }
            }
        }
    }
}
