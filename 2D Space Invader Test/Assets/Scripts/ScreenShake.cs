using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public Animator screenShakeAnimator;

    public void TriggerScreenShake() {
        screenShakeAnimator.SetTrigger("ScreenShake");
    }
}
