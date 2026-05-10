using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerPersist.instance != null)
            {
                HealthSystem hs = Object.FindFirstObjectByType<HealthSystem>();
                if (hs != null)
                    PlayerPersist.instance.savedHealth = hs.health;

                PlayerPersist.instance.savedStarCount = GameManager.Instance.currentStars;
                PlayerPersist.instance.hasData = true; 
            }

            PlayerPrefs.SetInt("Restarted", 1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
