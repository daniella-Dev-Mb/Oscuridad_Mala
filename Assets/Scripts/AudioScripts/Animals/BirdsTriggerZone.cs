using UnityEngine;

public class BirdsTriggerZone : MonoBehaviour
{
    public BirdsAudioClips bushAudio;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bushAudio.PlayRandomSound();

            gameObject.SetActive(false);
        }
    }
}
