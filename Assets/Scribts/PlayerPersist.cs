using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPersist : MonoBehaviour
{
    public static PlayerPersist instance;

    public int savedHealth = 3;
    public Vector3 savedPosition;
    public bool hasData = false;
    public List<string> collectedStars = new List<string>();
    public List<string> killedEnemies = new List<string>();
    public int savedStarCount = 0;

    [Header("Background Music")]
    public AudioClip backgroundMusic;
    public AudioSource musicSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.volume = 1f;
            musicSource.Play();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Boss Level")
        {
            if (musicSource != null)
                musicSource.Stop();
        }
        else
        {
            if (musicSource != null && !musicSource.isPlaying)
                musicSource.Play();
        }

        if (scene.name == "Boss Level")
        {
            IntroCutscene intro = GetComponent<IntroCutscene>();
            if (intro != null)
            {
                intro.enabled = false;
                intro.enabled = true;
            }
        }
    }
}