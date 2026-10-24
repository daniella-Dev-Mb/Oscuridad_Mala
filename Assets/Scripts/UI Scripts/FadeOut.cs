using UnityEngine;

public class FadeOut : MonoBehaviour
{
    public Animator animationFade;

    public void FadeMenu()
    {
        animationFade.SetBool("FadeTransition", true);
    }

    public void FadeReset()
    {
        animationFade.SetBool("FadeReset", true);
    }
}
