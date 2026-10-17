using UnityEngine;

public class AudioTrigger : MonoBehaviour
{

    public BushAudioClips bushAudio;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bushAudio.PlayRandomSound();
        }
    }

}
