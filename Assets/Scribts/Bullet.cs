using UnityEngine;
using static Unity.VisualScripting.Member;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    private Vector2 direction;
    public ParticleSystem explosionparticale;
    public AudioClip deathSound;

    void Start()
    {
        Destroy(gameObject,5f);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Instantiate(explosionparticale, transform.position,
                        explosionparticale.transform.rotation);

            if (deathSound != null)
            {
                AudioSource source = new GameObject("DeathSound").AddComponent<AudioSource>();
                source.clip = deathSound;
                source.volume = 0.2f;
                source.ignoreListenerVolume = true;
                source.Play();
                Destroy(source.gameObject, deathSound.length);
            }

            EnemyThrow enemyThrow = other.gameObject.GetComponent<EnemyThrow>();
            if (enemyThrow != null)
            {
                enemyThrow.TakeDamage(1);
            }
            else
            {
                if (PlayerPersist.instance != null)
                    PlayerPersist.instance.killedEnemies.Add(other.gameObject.name);
                Destroy(other.gameObject);
            }

            Destroy(gameObject);
        }
    }
}