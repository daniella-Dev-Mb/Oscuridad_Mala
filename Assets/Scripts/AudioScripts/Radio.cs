using UnityEngine;

public class Radio : MonoBehaviour
{
    public AudioSource source;
    public AudioSource buttonSource;
    public AudioClip initialClip;
    ///////////placeholder
    public AudioClip changedClip; // Audio después de pulsar el botón, pendiente.
    ///////////placeholder
    public AudioClip buttonClip; // Asignar cuando el equipo confirme el sonido.
    public bool IsOn { get; private set; }
    bool changed;

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
        return changedClip == null ? "Radio encendida - audio pendiente" : "E: cambiar audio";
    }

    public void Interact(PlayerController player)
    {
        if (player == null || player.IsDefeated || source == null) return;
        if (!IsOn)
        {
            if (player.pilas <= 0) return;
            player.pilas--;
            IsOn = true;
            source.clip = initialClip;
            source.loop = true;
            if (source.clip != null) source.Play();
            return;
        }
        if (buttonSource != null && buttonClip != null) buttonSource.PlayOneShot(buttonClip);
        ///////////placeholder
        // Si falta el siguiente audio, mantenemos el actual.
        if (changedClip == null) return;
        changed = !changed;
        source.clip = changed ? changedClip : initialClip;
        if (source.clip != null) source.Play();
        else source.Stop();
    }
}
