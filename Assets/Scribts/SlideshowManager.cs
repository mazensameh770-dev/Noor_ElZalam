using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SlideshowManager : MonoBehaviour
{
    public static SlideshowManager instance;

    public GameObject slideshowPanel;
    public Image[] slideshowImages;
    public AudioClip victoryMusic;
    public AudioSource victoryAudioSource;
    public GameObject youWinPanel;

    void Awake()
    {
        instance = this;
    }

    public void StartSlideshow()
    {
        StartCoroutine(PlaySlideshow());
    }

    IEnumerator PlaySlideshow()
    {
        foreach (var img in slideshowImages)
            img.gameObject.SetActive(false);

        if (victoryAudioSource != null && victoryMusic != null)
        {
            victoryAudioSource.clip = victoryMusic;
            victoryAudioSource.loop = true;
            victoryAudioSource.Play();
        }

        slideshowPanel.SetActive(true);
        youWinPanel.SetActive(false);

        for (int i = 0; i < slideshowImages.Length; i++)
        {
            slideshowImages[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(2f);
            if (i < slideshowImages.Length - 1)
                slideshowImages[i].gameObject.SetActive(false);
        }

        youWinPanel.SetActive(true);
    }
}