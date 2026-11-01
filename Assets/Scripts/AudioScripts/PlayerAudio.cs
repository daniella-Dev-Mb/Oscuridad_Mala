using UnityEngine;

[RequireComponent(typeof(PlayerController), typeof(CharacterController))]
public class PlayerAudio : MonoBehaviour
{
    public AudioSource footstepsSource;
    public AudioSource lanternSource;
    public AudioSource ignitionSource;
    public AudioClip[] footsteps;
    public AudioClip ignitionClip;
    public AudioClip refillClip;
    ///////////placeholder
    public AudioClip extinguishClip;

    public void PlayRefill()
    {
        if (ignitionSource != null && refillClip != null) ignitionSource.PlayOneShot(refillClip);
    }

    public void PlayExtinguish()
    {
        if (extinguishClip == null) return;
        GameObject sound = new GameObject("Apagado linterna");
        AudioSource source = sound.AddComponent<AudioSource>();
        if (ignitionSource != null) source.outputAudioMixerGroup = ignitionSource.outputAudioMixerGroup;
        source.clip = extinguishClip;
        source.spatialBlend = 0f;
        DontDestroyOnLoad(sound);
        source.Play();
        Destroy(sound, extinguishClip.length);
    }

    [Min(0.01f)] public float walkStepDistance = 1.8f;
    [Min(0.01f)] public float runStepDistance = 2.2f;
    [Min(0f)] public float minimumSpeed = 0.1f;
    [Range(0f, 1f)] public float walkVolume = 0.45f;
    [Range(0f, 1f)] public float runVolume = 0.75f;
    public Vector2 walkPitch = new Vector2(0.95f, 1.05f);
    public Vector2 runPitch = new Vector2(1.05f, 1.15f);
    [Range(0f, 1f)] public float lanternVolume = 0.2f;

    PlayerController player;
    CharacterController body;
    float travelled;
    int lastStep = -1;

    void Awake()
    {
        player = GetComponent<PlayerController>();
        body = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        if (ignitionSource != null && ignitionClip != null)
            ignitionSource.PlayOneShot(ignitionClip);
        if (lanternSource != null && lanternSource.clip != null)
        {
            lanternSource.volume = lanternVolume * player.LanternCharge;
            lanternSource.Play();
        }
    }

    void LateUpdate()
    {
        bool paused = Time.deltaTime <= 0f || player.IsDefeated;
        if (lanternSource != null)
        {
            lanternSource.volume = lanternVolume * player.LanternCharge;
            if (paused) lanternSource.Pause();
            else lanternSource.UnPause();
        }
        Vector3 velocity = body.velocity;
        velocity.y = 0f;
        float speed = velocity.magnitude;
        if (paused || !body.isGrounded || speed <= minimumSpeed)
        {
            travelled = 0f;
            if (footstepsSource != null) footstepsSource.Stop();
            return;
        }
        if (footstepsSource == null || footsteps == null || footsteps.Length == 0) return;

        float running = Mathf.InverseLerp(player.walkSpeed, player.runSpeed, speed);
        travelled += speed * Time.deltaTime;
        float stepDistance = Mathf.Max(0.01f, Mathf.Lerp(walkStepDistance, runStepDistance, running));
        if (travelled < stepDistance) return;
        travelled %= stepDistance;
        int index = Random.Range(0, footsteps.Length - (lastStep >= 0 && footsteps.Length > 1 ? 1 : 0));
        if (footsteps.Length > 1 && index >= lastStep && lastStep >= 0) index++;
        lastStep = index;
        footstepsSource.volume = Mathf.Lerp(walkVolume, runVolume, running);
        footstepsSource.pitch = Random.Range(Mathf.Lerp(walkPitch.x, runPitch.x, running),
            Mathf.Lerp(walkPitch.y, runPitch.y, running));
        footstepsSource.PlayOneShot(footsteps[index]);
    }

    void OnDisable()
    {
        travelled = 0f;
        if (footstepsSource != null) footstepsSource.Stop();
        if (lanternSource != null) lanternSource.Stop();
        if (ignitionSource != null) ignitionSource.Stop();
    }
}
