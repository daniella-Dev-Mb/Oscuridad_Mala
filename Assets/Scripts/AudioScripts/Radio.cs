using UnityEngine;
using UnityEngine.Events;

public class Radio : MonoBehaviour
{
    public AudioSource source;
    public AudioSource buttonSource;
    public AudioClip initialClip;
    public AudioClip changedClip;
    public AudioClip buttonClip;
    public AudioClip[] buttonClips = new AudioClip[0];
    public AudioClip[] channels = new AudioClip[0];
    public DialoguePlayer dialogue;
    public AudioClip broadcast;
    [Min(0f)] public float broadcastEndTime;
    public SubtitleLine[] broadcastSubtitles = new SubtitleLine[0];
    public UnityEvent onBroadcastFinished = new UnityEvent();
    public bool IsOn { get; private set; }
    public bool HasBroadcast { get; private set; }
    bool broadcastPending;
    bool paused;
    bool PlayingBroadcast => source != null && source.clip == broadcast && (source.isPlaying || paused);
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
        if (broadcastPending || PlayingBroadcast) return "Escuchando radio...";
        if (broadcast != null) return "E: escuchar radio";
        return channels.Length == 0 && changedClip == null ? "Radio encendida - audio pendiente" : "E: cambiar emisora";
    }

    void Update()
    {
        if (source == null || !IsOn) return;
        if (dialogue != null && dialogue.player != null && dialogue.player.IsDefeated)
        {
            source.Stop();
            return;
        }
        if (Time.timeScale <= 0f)
        {
            if (!paused) { source.Pause(); paused = true; }
            return;
        }
        if (paused) { source.UnPause(); paused = false; }
        if (IsOn && !broadcastPending && source != null && !source.loop && !source.isPlaying && Time.timeScale > 0f)
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
        if (broadcastPending || PlayingBroadcast) return;
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
        if (dialogue != null && broadcast != null)
        {
            var finished = new UnityEvent();
            finished.AddListener(FinishBroadcast);
            broadcastPending = dialogue.Play(broadcast, broadcastSubtitles, "Radio", finished, source,
                broadcastEndTime, broadcastEndTime > 0f);
            return;
        }
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

    void FinishBroadcast()
    {
        broadcastPending = false;
        if (broadcastEndTime <= 0f) PlayStatic();
        if (HasBroadcast) return;
        HasBroadcast = true;
        onBroadcastFinished.Invoke();
    }

    void OnDisable()
    {
        if (source != null) source.Stop();
        if (buttonSource != null) buttonSource.Stop();
    }
}
