using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class BushAudioClips : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] clipList;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRandomSound()
    {
        if (clipList.Length == 0) return;
        
        int random = Random.Range(0, clipList.Length);

        audioSource.PlayOneShot (clipList[random]);
    }

}
