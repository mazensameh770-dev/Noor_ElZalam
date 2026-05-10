using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Move")]
    public float moveSpeed = 5f;
    public float forceAmount = 10f;

    [Header("Player Pos")]
    public Transform StartPoint;

    private Rigidbody2D RB;
    private bool isGrounded;

    [Header("Animator")]
    private Animator Animator;

    [Header("Bullet")]
    public GameObject BulletPrefab;
    public Transform firepoint;

    public bool isKnocked = false;
    SpriteRenderer SR;

    public GameObject fire;
    public GameObject Firepoint;
    float fireRate = 0.3f;   
    float nextFireTime = 0f;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (PlayerPersist.instance != null && PlayerPersist.instance.hasData)
        {
            GameObject spawn = GameObject.FindWithTag("Respawn");
            if (spawn != null)
                transform.position = spawn.transform.position;

            HealthSystem hs = Object.FindFirstObjectByType<HealthSystem>();
            if (hs != null)
                hs.health = PlayerPersist.instance.savedHealth;
        }
        else
        {
            GameObject spawn = GameObject.FindWithTag("Respawn");
            if (spawn != null)
                transform.position = spawn.transform.position;
            else if (StartPoint != null)
                transform.position = StartPoint.position;
        }
    }

    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        RB = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
    }
    void Update()
    {
        Move();
        Jump();
        Shoot();

        if (transform.position.y < -6)
        {
            BossFloor bf = Object.FindFirstObjectByType<BossFloor>();

            if (bf == null)
            {
                HealthSystem hs = Object.FindFirstObjectByType<HealthSystem>();
                if (hs != null)
                    hs.TakeDamage(hs.health);
                if (StartPoint != null)
                    transform.position = StartPoint.position;
                return;
            }
            if (bf == null)
                return;

            HealthSystem hs2 = Object.FindFirstObjectByType<HealthSystem>();
            if (hs2 != null)
                hs2.TakeDamage(hs2.health);
            if (StartPoint != null)
                transform.position = StartPoint.position;
        }
    }
    void Move()
    {
        if (isKnocked) return;

        float move = Input.GetAxis("Horizontal");
        RB.linearVelocity = new Vector2(move * moveSpeed, RB.linearVelocity.y);
        if (move != 0)
        {
            Animator.SetInteger("Walking", 6);
            if (move > 0)
                SR.flipX = false;
            else
                SR.flipX = true;
        }
        else
            Animator.SetInteger("Walking", 0);
    }
    void Jump()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input .GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && isGrounded)
        {
            RB.AddForce(Vector2.up * forceAmount, ForceMode2D.Impulse);
            isGrounded = false;
            Animator.SetTrigger("Jump");
        }
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    void Shoot()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.LeftControl)) && Time.time >= nextFireTime && Time.timeScale > 0)
        {
            nextFireTime = Time.time + fireRate;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - firepoint.position);

            GameObject bullet = Instantiate(BulletPrefab, firepoint.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().SetDirection(direction);

            StartCoroutine(FireEffect());
        }
    }
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public IEnumerator KnockbackTime()
    {
        isKnocked = true;
        yield return new WaitForSeconds(0.2f);
        isKnocked = false;
    }

    IEnumerator FireEffect()
    { 
        Firepoint.SetActive(true);
        fire.SetActive(true);     // تظهر
        yield return new WaitForSeconds(0.4f); 
        Firepoint.SetActive(false);
        fire.SetActive(false);    
    }
}