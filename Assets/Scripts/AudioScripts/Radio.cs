using UnityEngine;

public class Radio : MonoBehaviour
{
    public AudioSource source;
    public AudioSource buttonSource;
    public AudioClip initialClip;
    public AudioClip changedClip;
    public AudioClip buttonClip;
    public AudioClip[] buttonClips = new AudioClip[0];
    public AudioClip[] channels = new AudioClip[0];
    public bool IsOn { get; private set; }
    int channel = -1;
    int nextButton;

    void Awake()
    {
        if (source != null)
        {
            source.playOnAwake = false;
            source.Stop();
        }
    }

    public string GetMessage(PlayerController player)
    {
        if (!IsOn) return player.pilas > 0 ? "E: poner pila y encender" : "Necesitás una pila";
        return channels.Length == 0 && changedClip == null ? "Radio encendida - audio pendiente" : "E: cambiar emisora";
    }

    void Update()
    {
        if (IsOn && source != null && !source.loop && !source.isPlaying && Time.timeScale > 0f)
            PlayStatic();
    }

    void PlayStatic()
    {
        source.clip = initialClip;
        source.loop = true;
        if (source.clip != null) source.Play();
        else source.Stop();
    }

    void PlayButton()
    {
        if (buttonSource == null) return;
        AudioClip clip = buttonClip;
        if (buttonClips.Length > 0)
        {
            clip = buttonClips[nextButton];
            nextButton = (nextButton + 1) % buttonClips.Length;
        }
        if (clip != null) buttonSource.PlayOneShot(clip);
    }

    public void Interact(PlayerController player)
    {
        if (player == null || player.IsDefeated || source == null) return;
        if (!IsOn)
        {
            if (player.pilas <= 0) return;
            player.pilas--;
            IsOn = true;
            PlayButton();
            PlayStatic();
            return;
        }
        PlayButton();
        if (channels.Length > 0)
        {
            channel = (channel + 1) % channels.Length;
            if (channels[channel] == null) return;
            source.clip = channels[channel];
        }
        else
        {
            if (changedClip == null) return;
            source.clip = changedClip;
        }
        source.loop = false;
        if (source.clip != null) source.Play();
        else source.Stop();
    }
}
