using UnityEngine;

public class BirdsTriggerZone : MonoBehaviour
{
    public BirdsAudioClips bushAudio;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            bushAudio.PlayRandomSound();

            gameObject.SetActive(false);
        }
    }
}
