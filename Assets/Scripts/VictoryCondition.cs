using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryCondition : MonoBehaviour
{
    public Radio requiredRadio;
    public SoundTrigger requiredThought;
    public Collider barrier;
    public Renderer barrierRenderer;
    public DialoguePlayer dialogue;
    public bool finishGame = true;
    bool finished;

    public bool Unlocked => (requiredRadio == null || requiredRadio.HasBroadcast) &&
        (requiredThought == null || requiredThought.Finished);

    void Update()
    {
        if (barrier != null) barrier.enabled = !Unlocked;
        if (barrierRenderer != null) barrierRenderer.enabled = !Unlocked;
    }

    void OnTriggerEnter(Collider other)
    {
        TryFinish(other);
    }

    void OnTriggerStay(Collider other)
    {
        TryFinish(other);
    }

    void TryFinish(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (!isActiveAndEnabled || !finishGame || finished || !Unlocked || player == null || player.IsDefeated) return;
        finished = true;
        if (dialogue != null) dialogue.FinishGame();
        else SceneManager.LoadScene("WinScene");
    }
}
