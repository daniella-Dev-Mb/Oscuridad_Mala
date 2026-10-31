using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TriggeredBySound : MonoBehaviour
{

    public AudioSource audioSource;

    public UnityEvent onSoundEnd;

    void Start()
    {
        TriggerScene();
    }

    public void TriggerScene()
    {
        StartCoroutine(ChangeSceneAfterSound());
     
    }

    private IEnumerator ChangeSceneAfterSound()
    {
        audioSource.Play();

        yield return new WaitForSeconds(audioSource.clip.length);

        onSoundEnd.Invoke();
    }
}
