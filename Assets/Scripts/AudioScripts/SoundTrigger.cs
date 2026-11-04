using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class SoundTrigger : MonoBehaviour
{
    public AudioSource source;
    ///////////placeholder
    public AudioClip clip;
    public DialoguePlayer dialogue;
    public SubtitleLine[] subtitles = new SubtitleLine[0];
    public string speaker = "Voz";
    public SoundTrigger prerequisite;
    public Radio requiredRadio;
    public bool playOnExit;
    public bool playPrerequisiteOnExit;
    public UnityEvent onFinished = new UnityEvent();
    public bool Played { get; private set; }
    public bool Finished { get; private set; }
    bool exited;

    void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
        if (source != null) source.playOnAwake = false;
    }

    bool IsPlayer(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        return player != null && player.isActiveAndEnabled && !player.IsDefeated;
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other)) exited = false;
        if (!playOnExit && IsPlayer(other)) Play();
    }

    void OnTriggerStay(Collider other)
    {
        if (!playOnExit && IsPlayer(other)) Play();
    }

    void OnTriggerExit(Collider other)
    {
        if (playOnExit && IsPlayer(other))
        {
            exited = true;
            if (playPrerequisiteOnExit && prerequisite != null) prerequisite.Play();
            Play();
        }
    }

    void Update()
    {
        if (playOnExit && exited && !Played) Play();
    }

    public void Play()
    {
        if (Played || clip == null || (prerequisite != null && !prerequisite.Played)) return;
        if (requiredRadio != null && !requiredRadio.HasBroadcast) return;
        if (dialogue != null)
        {
            var finished = new UnityEvent();
            finished.AddListener(Finish);
            Played = dialogue.Play(clip, subtitles, speaker, finished);
        }
        else if (source != null)
        {
            source.PlayOneShot(clip);
            Played = true;
            Invoke(nameof(Finish), clip.length);
        }
    }

    void Finish()
    {
        Finished = true;
        onFinished.Invoke();
    }
}
