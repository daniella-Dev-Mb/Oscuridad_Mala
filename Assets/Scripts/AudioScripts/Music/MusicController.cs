
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
    public static MusicController Instance { get; private set; }

    [Header("Snapshots")]
    [SerializeField] private AudioMixerSnapshot mainMenu;
    [SerializeField] private AudioMixerSnapshot bonfires;
    [SerializeField] private AudioMixerSnapshot exploration;
    [SerializeField] private AudioMixerSnapshot camp;
    [SerializeField] private AudioMixerSnapshot win;
    [SerializeField] private AudioMixerSnapshot lose;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource mainMenuSource;
    [SerializeField] private AudioSource bonfiresSource;
    [SerializeField] private AudioSource explorationSource;
    [SerializeField] private AudioSource campSource;
    [SerializeField] private AudioSource winSource;
    [SerializeField] private AudioSource loseSource;

    [Header("Nombres de las escenas")]
    [SerializeField] private string menuScene = "MainMenu";
    [SerializeField] private string gameScene = "Game";
    [SerializeField] private string winScene = "Win";
    [SerializeField] private string loseScene = "Lose";

    [Header("Duracion de transiciones")]
    [SerializeField] private float normalFade = 2f;
    [SerializeField] private float campFade = 4f;
    [SerializeField] private float winFade = 3f;
    [SerializeField] private float loseFade = 1f;

    private bool isInCamp;
    private bool gameFinished;

    private AudioSource currentSource;
    private Coroutine stopCoroutine;

    private AudioSource[] sources;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        sources = new AudioSource[]
        {
            mainMenuSource,
            bonfiresSource,
            explorationSource,
            campSource,
            winSource,
            loseSource
        };

        foreach (AudioSource source in sources)
        {
            if (source == null)
                continue;

            source.loop = true;
            source.playOnAwake = false;
            source.Stop();
        }

        SceneManager.sceneLoaded += OnSceneLoaded;

        ApplySceneMusic(
            SceneManager.GetActiveScene().name,
            true
        );
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        Instance = null;
    }

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode)
    {
        ApplySceneMusic(scene.name, false);
    }

    private void ApplySceneMusic(
        string sceneName,
        bool instant)
    {
        if (sceneName == menuScene)
        {
            isInCamp = false;
            gameFinished = false;

            ChangeMusic(
                mainMenu,
                mainMenuSource,
                instant ? 0f : normalFade
            );
        }
        else if (sceneName == gameScene)
        {
            isInCamp = false;
            gameFinished = false;

            ChangeMusic(
                bonfires,
                bonfiresSource,
                instant ? 0f : normalFade
            );
        }
        else if (sceneName == winScene)
        {
            WinGame();
        }
        else if (sceneName == loseScene)
        {
            LoseGame();
        }
    }

    private void ChangeMusic(
        AudioMixerSnapshot snapshot,
        AudioSource newSource,
        float fadeTime)
    {
        if (snapshot == null || newSource == null)
        {
            Debug.LogWarning(
                "Falta configurar un Snapshot o AudioSource."
            );
            return;
        }

        if (newSource.clip == null)
        {
            Debug.LogWarning(
                "El AudioSource no tiene AudioClip."
            );
            return;
        }

        if (currentSource == newSource)
            return;

        if (stopCoroutine != null)
        {
            StopCoroutine(stopCoroutine);
            stopCoroutine = null;
        }

        newSource.Stop();
        newSource.time = 0f;

        snapshot.TransitionTo(fadeTime);

        newSource.Play();

        currentSource = newSource;

        if (fadeTime <= 0f)
        {
            StopInactiveSources();
        }
        else
        {
            stopCoroutine = StartCoroutine(
                StopAfterFade(fadeTime)
            );
        }
    }

    private IEnumerator StopAfterFade(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        StopInactiveSources();
        stopCoroutine = null;
    }

    private void StopInactiveSources()
    {
        foreach (AudioSource source in sources)
        {
            if (source == null || source == currentSource)
                continue;

            if (source.isPlaying)
                source.Stop();
        }
    }

    public void EnterBonfire()
    {
        if (gameFinished || isInCamp)
            return;

        ChangeMusic(
            bonfires,
            bonfiresSource,
            normalFade
        );
    }

    public void ExitBonfire()
    {
        if (gameFinished || isInCamp)
            return;

        ChangeMusic(
            exploration,
            explorationSource,
            normalFade
        );
    }

    public void EnterCamp()
    {
        if (gameFinished || isInCamp)
            return;

        isInCamp = true;

        ChangeMusic(
            camp,
            campSource,
            campFade
        );
    }

    public void WinGame()
    {
        if (gameFinished)
            return;

        gameFinished = true;

        ChangeMusic(
            win,
            winSource,
            winFade
        );
    }

    public void LoseGame()
    {
        if (gameFinished)
            return;

        gameFinished = true;

        ChangeMusic(
            lose,
            loseSource,
            loseFade
        );
    }
}
