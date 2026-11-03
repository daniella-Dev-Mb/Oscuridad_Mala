
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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        AudioSource[] sources =
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
            if (source == null || source.clip == null)
                continue;

            source.loop = true;
            source.playOnAwake = false;
            source.Play();
        }

        ApplySceneMusic(SceneManager.GetActiveScene().name, true);
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        Instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplySceneMusic(scene.name, false);
    }

    private void ApplySceneMusic(string sceneName, bool instant)
    {
        if (sceneName == menuScene)
        {
            isInCamp = false;
            gameFinished = false;

            mainMenu.TransitionTo(instant ? 0f : normalFade);
        }
        else if (sceneName == gameScene)
        {
            isInCamp = false;
            gameFinished = false;

            bonfires.TransitionTo(instant ? 0f : normalFade);
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

    public void EnterBonfire()
    {
        if (gameFinished || isInCamp)
            return;

        bonfires.TransitionTo(normalFade);
    }

    public void ExitBonfire()
    {
        if (gameFinished || isInCamp)
            return;

        exploration.TransitionTo(normalFade);
    }

    public void EnterCamp()
    {
        if (gameFinished || isInCamp)
            return;

        isInCamp = true;
        camp.TransitionTo(campFade);
    }

    public void WinGame()
    {
        if (gameFinished)
            return;

        gameFinished = true;
        win.TransitionTo(winFade);
    }

    public void LoseGame()
    {
        if (gameFinished)
            return;

        gameFinished = true;
        lose.TransitionTo(loseFade);
    }
}
