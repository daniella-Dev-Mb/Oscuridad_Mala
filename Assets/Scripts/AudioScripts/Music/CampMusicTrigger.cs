using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CampMusicTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (MusicController.Instance != null)
            MusicController.Instance.EnterCamp();
    }
}
