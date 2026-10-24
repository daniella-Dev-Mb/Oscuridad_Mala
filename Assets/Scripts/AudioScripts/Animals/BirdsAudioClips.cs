using UnityEngine;

public class BirdsAudioClips : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] clipList;
    int lastClip = -1;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRandomSound()
    {
        if (audioSource == null || clipList == null || clipList.Length == 0) return;

        int random = Random.Range(0, clipList.Length - (lastClip >= 0 && clipList.Length > 1 ? 1 : 0));
        if (clipList.Length > 1 && lastClip >= 0 && random >= lastClip) random++;
        lastClip = random;

        audioSource.PlayOneShot(clipList[random]);
    }
}
