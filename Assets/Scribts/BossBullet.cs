using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Hit(other.gameObject, other.transform, other.GetComponent<Rigidbody2D>());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Hit(collision.gameObject, collision.transform, collision.rigidbody);
    }

    void Hit(GameObject obj, Transform objTransform, Rigidbody2D rb)
    {
        if (obj.CompareTag("Player"))
        {
            HealthSystem health = FindFirstObjectByType<HealthSystem>();
            if (health != null)
                health.TakeDamage(1);

            if (rb != null)
            {
                Vector2 dir = (objTransform.position - transform.position).normalized;
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(dir * 10f, ForceMode2D.Impulse);

                PlayerMovement pm = obj.GetComponent<PlayerMovement>();
                if (pm != null)
                    pm.StartCoroutine(pm.KnockbackTime());
            }

            Destroy(gameObject);
        }
    }
}