using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SoundTrigger : MonoBehaviour
{
    public AudioSource source;
    ///////////placeholder
    public AudioClip clip;
    public bool Played { get; private set; }

    void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
        if (source != null) source.playOnAwake = false;
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null && player.isActiveAndEnabled && !player.IsDefeated) Play();
    }

    public void Play()
    {
        if (Played || source == null || clip == null) return;
        source.PlayOneShot(clip);
        Played = true;
    }
}
