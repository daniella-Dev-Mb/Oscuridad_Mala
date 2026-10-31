using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class WolfTriggerZone : MonoBehaviour
{

    [SerializeField] private AudioSource wolfAudio;
    [SerializeField] private AudioClip wolfClip;

    private void OnTriggerEnter (Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (wolfAudio != null && wolfClip != null)
            {
                wolfAudio.PlayOneShot (wolfClip);
            }
        }
    }

}
