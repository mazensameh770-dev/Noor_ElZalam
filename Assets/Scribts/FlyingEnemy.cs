using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class FlyingEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 4f;
    public float stopDistance = 0.5f;
    public float chaseRange = 10f;

    [Header("Animator")]
    private Animator Animator;

    [Header("Game Over")]
    public float delayBeforeRestart = 1f;

    private Transform player;
    private bool gameOver = false;

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
        if (player == null || gameOver) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < chaseRange && distance > stopDistance)
        {
            MoveToPlayer();
        }

        Flip();
    }

    void MoveToPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void Flip()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(0.5f, 0.5f, 1);
        else
            transform.localScale = new Vector3(-0.5f, 0.5f, 1);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !gameOver)
        {
            gameOver = true;
            Animator.SetBool("Attack", true);

            HealthSystem hs = Object.FindFirstObjectByType<HealthSystem>();
            if (hs != null)
                hs.TakeDamage(1);

            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 direction = (other.transform.position - transform.position).normalized;
                float knockbackForce = 10f;

                playerRb.linearVelocity = Vector2.zero;
                playerRb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);

                PlayerMovement pm = other.GetComponent<PlayerMovement>();
                if (pm != null)
                    pm.StartCoroutine(pm.KnockbackTime());
            }

            gameOver = false;
        }
    }

}