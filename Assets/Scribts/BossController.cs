using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;
    private bool isInvincible = false;

    [Header("Health Bar UI")]
    public UnityEngine.UI.Slider healBar;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float initialFireRate = 2f;
    public float fireRateIncreaseInterval = 10f; 
    public float fireRateMultiplier = 0.85f;     
    public float projectileSpeed = 6f;
    public float projectileSpeedIncrease = 1f;   

    private float currentFireRate;
    private float currentProjectileSpeed;
    private Transform player;

    [Header("Phase 2 - Mini Enemy Spawn")]
    public GameObject miniBossEnemyPrefab;
    public Transform[] spawnPoints;
    public float miniEnemyBaseSpeed = 3f;
    public float miniEnemySpeedIncrease = 0.5f;
    private bool isPhase2 = false;
    private float currentMiniEnemySpeed;

    [Header("Invincibility")]
    public float invincibleDuration = 5f;

    private Animator anim;

    [Header("Battle Audio")]
    public AudioClip battleMusic;
    public AudioSource musicSource;

    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = maxHealth;
        currentFireRate = initialFireRate;
        currentProjectileSpeed = projectileSpeed;
        currentMiniEnemySpeed = miniEnemyBaseSpeed;
        UpdateHealthBar();
    }

    public void StartBossFight()
    {
        StopAllCoroutines();
        StartCoroutine(ShootingLoop());
        StartCoroutine(IncreaseFireRateLoop());
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = battleMusic;
        musicSource.volume = 1.0f;
        musicSource.Play();
        musicSource.volume =AudioListener.volume;
    }

    IEnumerator ShootingLoop()
    {
        yield return new WaitForSeconds(1f);

        while (!isDead)
        {
            Debug.Log("ShootingLoop شغال - isDead: " + isDead + " isInvincible: " + isInvincible);

            if (!isInvincible)
                Shoot();

            yield return new WaitForSeconds(currentFireRate);
        }

        Debug.Log("ShootingLoop وقف!");
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null || player == null) return;

        if (anim != null)
        {
            anim.ResetTrigger("isAttacking");
            anim.SetTrigger("isAttacking");
        }
        
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // اتجاه الطلقة ناحية اللاعب
        Vector2 direction = (player.position - firePoint.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction * currentProjectileSpeed;
    }

    IEnumerator IncreaseFireRateLoop()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(fireRateIncreaseInterval);

            currentFireRate *= fireRateMultiplier;
            currentFireRate = Mathf.Max(currentFireRate, 0.3f); 

            currentProjectileSpeed += projectileSpeedIncrease;

            currentMiniEnemySpeed += miniEnemySpeedIncrease;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateHealthBar();

        if (!isPhase2 && currentHealth <= maxHealth / 2)
        {
            isPhase2 = true;
            StartCoroutine(Phase2Transition());
            return;
        }

        if (currentHealth <= 0 && isPhase2)
        {
            StartCoroutine(Death2());
        }
    }

    IEnumerator Phase2Transition()
    {
        isInvincible = true;

        StartCoroutine(FlashEffect());

        yield return new WaitForSeconds(invincibleDuration);

        isInvincible = false;

        GetComponent<SpriteRenderer>().color = Color.white;

        StartCoroutine(SpawnMiniEnemiesLoop());

        StartCoroutine(ShootingLoop());
    }

    IEnumerator FlashEffect()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float elapsed = 0f;
        bool visible = true;

        while (elapsed < invincibleDuration)
        {
            visible = !visible;
            sr.color = visible ? Color.white : new Color(1f, 1f, 1f, 0.2f);
            elapsed += 0.15f;
            yield return new WaitForSeconds(0.15f);
        }

        sr.color = Color.white;
    }

    IEnumerator SpawnMiniEnemiesLoop()
    {
        if (miniBossEnemyPrefab == null || spawnPoints == null) yield break;

        while (!isDead)
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                GameObject miniEnemy = Instantiate(miniBossEnemyPrefab,
                                       spawnPoint.position, Quaternion.identity);

                var enemyScript = miniEnemy.GetComponent<FlyingEnemy>();
                if (enemyScript != null)
                    enemyScript.speed = currentMiniEnemySpeed;
            }

            yield return new WaitForSeconds(6f);
        }
    }

    IEnumerator Death2()
    {
        isDead = true;

        if (musicSource != null)
            musicSource.Stop();

        if (anim != null)
            anim.SetTrigger("death2");

        yield return new WaitForSeconds(3f);

        SlideshowManager.instance.StartSlideshow();

        gameObject.SetActive(false);
    }

   

    void UpdateHealthBar()
    {
        if (healBar != null)
            healBar.value = currentHealth;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            TakeDamage(2); 
            Destroy(other.gameObject);
        }
    }
}