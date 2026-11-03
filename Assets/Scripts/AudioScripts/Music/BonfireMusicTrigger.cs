using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BonfireMusicTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (MusicController.Instance != null)
            MusicController.Instance.EnterBonfire();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (MusicController.Instance != null)
            MusicController.Instance.ExitBonfire();
    }
}
