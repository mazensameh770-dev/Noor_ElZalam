using UnityEngine;
using UnityEngine.UI;


public class HealthSystem : MonoBehaviour
{
    public Image heart1;
    public Image heart2;
    public Image heart3;

    public int health = 3;

    public Color emptyColor = Color.black;
    public Color filledColor = Color.red;

    private bool isInvincible = false;
    public float invincibleTime = 1f;

    GameManager gameManager;

    void Start()
    {
            if (PlayerPersist.instance != null && PlayerPersist.instance.hasData)
            {
                health = PlayerPersist.instance.savedHealth;
            }

            UpdateHearts();
        gameManager = Object.FindFirstObjectByType<GameManager>();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        health -= damage;

        if (health < 0)
            health = 0;

        UpdateHearts();

        if (health == 0)
        {
            Die();
        }
    }

    void UpdateHearts()
    {
        heart1.color = (health >= 1) ? emptyColor : filledColor;
        heart2.color = (health >= 2) ? emptyColor : filledColor;
        heart3.color = (health >= 3) ? emptyColor : filledColor;
    }

    void Die()
    {
        if (PlayerPersist.instance != null)
        {
            PlayerPersist.instance.hasData = false;
            PlayerPersist.instance.savedHealth = 3;
        }

        if (gameManager != null)
            gameManager.GameOver();
    }
}