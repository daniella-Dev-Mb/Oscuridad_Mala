using UnityEngine;

public class BushAudioTrigger : MonoBehaviour
{

    public BushAudioClips bushAudio;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bushAudio.PlayRandomSound();

            gameObject.SetActive(false);
        }
    }

}
