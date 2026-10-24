using UnityEngine;

public class ForestAmbience : MonoBehaviour
{
    public AudioSource introSource;
    public AudioSource loopSource;
    public AudioClip introClip;
    public AudioClip loopClip;
    [Min(0.05f)] public float schedulingLead = 0.2f;

    void OnEnable()
    {
        if (introSource == null || loopSource == null || loopClip == null) return;
        double start = AudioSettings.dspTime + schedulingLead;
        loopSource.clip = loopClip;
        loopSource.loop = true;
        if (introClip != null)
        {
            introSource.clip = introClip;
            introSource.loop = false;
            introSource.PlayScheduled(start);
            // El reloj de audio evita un silencio entre la entrada con fade y el bucle.
            start += (double)introClip.samples / introClip.frequency;
        }
        loopSource.PlayScheduled(start);
    }

    void OnDisable()
    {
        if (introSource != null) introSource.Stop();
        if (loopSource != null) loopSource.Stop();
    }
}
