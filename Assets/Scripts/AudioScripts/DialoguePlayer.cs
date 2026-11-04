using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class SubtitleLine
{
    public float start;
    public float end;
    [TextArea] public string text;
}

public class DialoguePlayer : MonoBehaviour
{
    public PlayerController player;
    public AudioSource source;
    public GameObject subtitlePanel;
    public Text subtitleLabel;
    public string victoryScene = "WinScene";
    public Transform[] campfires = new Transform[0];
    public bool IsBusy { get; private set; }

    class Request
    {
        public AudioClip clip;
        public AudioSource source;
        public SubtitleLine[] lines;
        public string speaker;
        public UnityEvent finished;
        public float endTime;
        public bool continueAudio;
    }

    readonly Queue<Request> queue = new Queue<Request>();
    AudioSource currentSource;

    public bool Play(AudioClip clip, SubtitleLine[] lines, string speaker,
        UnityEvent finished, AudioSource audioSource = null, float endTime = 0f, bool continueAudio = false)
    {
        AudioSource target = audioSource != null ? audioSource : source;
        if (clip == null || target == null || player == null || player.IsDefeated) return false;
        queue.Enqueue(new Request { clip = clip, source = target, lines = lines,
            speaker = speaker, finished = finished, endTime = endTime, continueAudio = continueAudio });
        if (!IsBusy) StartCoroutine(PlayQueue());
        return true;
    }

    IEnumerator PlayQueue()
    {
        IsBusy = true;
        while (queue.Count > 0)
        {
            Request request = queue.Dequeue();
            if (request.source == null) continue;
            currentSource = request.source;
            currentSource.clip = request.clip;
            currentSource.loop = false;
            currentSource.Play();
            bool paused = false;
            while (currentSource != null && (currentSource.isPlaying || paused))
            {
                if (player == null || player.IsDefeated) break;
                if (Time.timeScale <= 0f && !paused) { currentSource.Pause(); paused = true; }
                else if (Time.timeScale > 0f && paused) { currentSource.UnPause(); paused = false; }
                if (!paused && request.endTime > 0f && currentSource.time >= request.endTime) break;
                string text = "";
                if (request.lines != null)
                {
                    foreach (SubtitleLine line in request.lines)
                    {
                        if (currentSource.time >= line.start && currentSource.time <= line.end)
                        {
                            text = line.text;
                            break;
                        }
                    }
                }
                if (subtitleLabel != null) subtitleLabel.text = string.IsNullOrEmpty(text) ? "" : request.speaker + ": " + text;
                if (subtitlePanel != null) subtitlePanel.SetActive(!string.IsNullOrEmpty(text));
                yield return null;
            }
            if (subtitlePanel != null) subtitlePanel.SetActive(false);
            if (currentSource != null && request.endTime > 0f && !request.continueAudio) currentSource.Stop();
            currentSource = null;
            if (player == null || player.IsDefeated) { queue.Clear(); break; }
            request.finished?.Invoke();
        }
        IsBusy = false;
    }

    public void FinishGame()
    {
        if (player == null || player.IsDefeated || !Application.CanStreamedLevelBeLoaded(victoryScene)) return;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(victoryScene);
    }

    void OnDisable()
    {
        StopAllCoroutines();
        queue.Clear();
        if (currentSource != null) currentSource.Stop();
        currentSource = null;
        IsBusy = false;
        if (subtitlePanel != null) subtitlePanel.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        for (int i = 1; i < campfires.Length; i++)
            if (campfires[i - 1] != null && campfires[i] != null)
                Gizmos.DrawLine(campfires[i - 1].position, campfires[i].position);
    }
}
