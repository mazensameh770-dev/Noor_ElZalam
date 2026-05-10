using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject titleScreen;
    public GameObject settingsPanel;
    public GameObject gameOverPanel;
    public GameObject HowToPlayPanel;

    [Header("Audio")]
    public AudioSource musicSource;
    public Slider volumeSlider;

    [Header("Stars")]
    public int currentStars = 0;

    [Header("Game Over Audio")]
    public AudioClip gameOverSound;
    private AudioSource gameOverSource;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        if (PlayerPersist.instance != null && PlayerPersist.instance.hasData)
        {
            currentStars = PlayerPersist.instance.savedStarCount;
        }
    
        
        if (PlayerPrefs.GetInt("Restarted") == 1)
        {
            Time.timeScale = 1f;
            PlayerPrefs.SetInt("Restarted", 0);
            titleScreen.SetActive(false);
            settingsPanel.SetActive(false);
            gameOverPanel.SetActive(false);
            HowToPlayPanel.SetActive(false);
        }
        else
        {
            Time.timeScale = 0f;
            titleScreen.SetActive(true);
            settingsPanel.SetActive(false);
            gameOverPanel.SetActive(false);
            HowToPlayPanel.SetActive(false);

            volumeSlider.value = musicSource.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        titleScreen.SetActive(false);
        settingsPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        HowToPlayPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        Time.timeScale = 0f;
        settingsPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        titleScreen.SetActive(false);
        HowToPlayPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        Time.timeScale = 1f;
        titleScreen.SetActive(false);
        settingsPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        HowToPlayPanel.SetActive(false);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        PlayerPrefs.SetInt("Restarted", 1);
        if (PlayerPersist.instance != null)
        {
            PlayerPersist.instance.hasData = false;
            PlayerPersist.instance.savedHealth = 3;
            PlayerPersist.instance.savedStarCount = 0;
            PlayerPersist.instance.collectedStars.Clear();
            PlayerPersist.instance.killedEnemies.Clear();
        }
        if (gameOverSource != null)
        {
            gameOverSource.Stop();
            Destroy(gameOverSource);
        }
        StartCoroutine(LoadAfterSound(0));
}

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        if (PlayerPersist.instance != null)
        {
            PlayerPersist.instance.hasData = false;
            PlayerPersist.instance.savedHealth = 3;
            PlayerPersist.instance.savedStarCount = 0;
            PlayerPersist.instance.collectedStars.Clear();
            PlayerPersist.instance.killedEnemies.Clear();
        }

        StartCoroutine(LoadAfterSound(0));
        if (gameOverSource != null)
        {
            gameOverSource.Stop();
            Destroy(gameOverSource);
        }
    }

    public void GameOver()
    {
        AudioListener.pause = false;

        if (PlayerPersist.instance != null &&
            PlayerPersist.instance.musicSource != null)
            PlayerPersist.instance.musicSource.Stop();

        BossController boss = FindFirstObjectByType<BossController>();
        if (boss != null && boss.musicSource != null)
        {
            boss.musicSource.Stop();
        }
        if (PlayerPersist.instance != null)
        {
            PlayerPersist.instance.hasData = false;
            PlayerPersist.instance.savedHealth = 3;
            PlayerPersist.instance.savedStarCount = 0;
            PlayerPersist.instance.collectedStars.Clear();
            PlayerPersist.instance.killedEnemies.Clear();
        }

        gameOverPanel.SetActive(true);

        if (gameOverSound != null && gameOverSource == null)
        {
            gameOverSource = gameObject.AddComponent<AudioSource>();
            gameOverSource.clip = gameOverSound;
            gameOverSource.loop = true;
            gameOverSource.volume = 0.2f;
            gameOverSource.Play();
        }
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;

        if (PlayerPersist.instance != null &&
            PlayerPersist.instance.musicSource != null)
            PlayerPersist.instance.musicSource.volume = volume;

        BossController boss = FindFirstObjectByType<BossController>();
        if (boss != null && boss.musicSource != null)
            boss.musicSource.volume = volume;
    }

    public void AddStars(int amount)
    {
        currentStars += amount;
    }

    public bool HasEnoughStars(int required)
    {
        return currentStars >= required;
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
    IEnumerator LoadAfterSound(int sceneIndex)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneIndex);
    }
    public void HowToPlay()
    {
        Time.timeScale = 0f;
        settingsPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        titleScreen.SetActive(false);
        HowToPlayPanel.SetActive(true);
    }
}
