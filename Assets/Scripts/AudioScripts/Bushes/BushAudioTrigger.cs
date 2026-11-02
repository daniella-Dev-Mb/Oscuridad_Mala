using UnityEngine;

public class BushAudioTrigger : MonoBehaviour
{

    public BushAudioClips bushAudio;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            bushAudio.PlayRandomSound();

            gameObject.SetActive(false);
        }
    }

}
