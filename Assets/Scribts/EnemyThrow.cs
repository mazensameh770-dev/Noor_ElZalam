using UnityEngine;
using System.Collections;

public class EnemyThrow : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 8f;
    public float fireRate = 2f;
    public float shootRange = 30f;

    [Header("Animator")]
    private Animator Animator;

    private Transform player;
    private float fireTimer = 0f;

    public int health = 1;

    void Start()
    {
        Animator = GetComponent<Animator>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (PlayerPersist.instance != null &&
            PlayerPersist.instance.killedEnemies.Contains(gameObject.name))
        {
            Destroy(gameObject);
        }

        if (p != null)
            player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        Flip();

        if (distance < shootRange)
        {
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireRate)
            {
                fireTimer = 0f;
                Shoot();
            }
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || player == null) return;

        Animator.SetBool("Attack", true);

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * bulletSpeed;
        }

        StartCoroutine(ResetAttackAnim());
    }

    IEnumerator ResetAttackAnim()
    {
        yield return new WaitForSeconds(0.3f);
        Animator.SetBool("Attack", false);
    }

    void Flip()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(0.5f, 0.5f, 1);
        else
            transform.localScale = new Vector3(-0.5f, 0.5f, 1);
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
            Die();
    }

    void Die()
    {
        if (PlayerPersist.instance != null)
            PlayerPersist.instance.killedEnemies.Add(gameObject.name);

        Destroy(gameObject);
    }
}