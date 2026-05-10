using UnityEngine;

public class StarCollect : MonoBehaviour
{
    public int Value = 1;
    public ParticleSystem explosionparticale;
    public AudioClip collectSound;
    void Start()
    {
        if (PlayerPersist.instance != null &&
            PlayerPersist.instance.collectedStars.Contains(gameObject.name))
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Instantiate(explosionparticale, transform.position,
                        explosionparticale.transform.rotation);

            if (collectSound != null)
            {
                AudioSource source = new GameObject("StarSound")
                                     .AddComponent<AudioSource>();
                source.clip = collectSound;
                source.volume = 0.5f;
                source.ignoreListenerVolume = true;
                source.Play();
                Destroy(source.gameObject, collectSound.length);
            }

            GameManager.Instance.AddStars(Value);
            int scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

            if ((scene == 0 && GameManager.Instance.currentStars == 3) ||
                (scene == 1 && GameManager.Instance.currentStars == 6) ||
                (scene == 2 && GameManager.Instance.currentStars == 9))
            {
                BorderMessage.ShowAllStarsMessage();
            }
            if (PlayerPersist.instance != null)
            {
                PlayerPersist.instance.collectedStars.Add(gameObject.name);
                PlayerPersist.instance.savedStarCount =
                    GameManager.Instance.currentStars;
            }

            gameObject.SetActive(false);
        }
    }
}
