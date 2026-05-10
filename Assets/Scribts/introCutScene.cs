using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class IntroCutscene : MonoBehaviour
{
    [Header("References")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;

    [Header("Boss Cutscene - اللي هييجي بعدين")]
    public BossIntroCutscene bossIntroCutscene; 

    [Header("Spawn Settings")]
    public float spawnYOffset = 10f;
    public float groundY = -3.5f;

    [Header("Timing")]
    public float fallDuration = 1.5f;
    public float lookDuration = 1f;
    public float messageDelay = 1f;

    [Header("Message")]
    [TextArea] public string introMessage = "What!........Where am I?";

    private SpriteRenderer sr;
    private Rigidbody2D rb;

    public GameObject firepoint;
    void Start()
    {
        if (firepoint != null)
        {
            firepoint.SetActive(false);
        }
        if (messagePanel == null)
        {
            GameObject bgCanvas = GameObject.Find("BackGroundCanvas");
            if (bgCanvas != null)
            {
                Transform found = bgCanvas.transform.Find("MessagePanel");
                if (found != null)
                    messagePanel = found.gameObject;
            }
        }
        if (messagePanel != null)
            messagePanel.SetActive(false);
    }
    void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (messagePanel == null)
        {
            GameObject bgCanvas = GameObject.Find("BackGroundCanvas");
            if (bgCanvas != null)
            {
                Transform found = bgCanvas.transform.Find("MessagePanel");
                if (found != null)
                    messagePanel = found.gameObject;
            }
        }

        if (messagePanel != null)
            messagePanel.SetActive(false);

        DisablePlayerControl(true);
        StopAllCoroutines();
        StartCoroutine(WaitAndStart());
    }

    IEnumerator WaitAndStart()
    {
        yield return null;
        yield return null;
        StartCoroutine(PlayIntroCutscene());
    }
    IEnumerator PlayIntroCutscene()
    {
        Vector3 startPos = transform.position;
        startPos.x = -5f;
        startPos.y = groundY + spawnYOffset;
        transform.position = startPos;

        Vector3 landPos = new Vector3(startPos.x, groundY, startPos.z);
        FaceLeft();

        float elapsed = 0f;
        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fallDuration;
            transform.position = Vector3.Lerp(startPos, landPos, t * t);
            yield return null;
        }
        transform.position = landPos;

        yield return StartCoroutine(LandingSquish());

        yield return new WaitForSeconds(0.2f);
        FaceRight();
        yield return new WaitForSeconds(lookDuration);
        FaceLeft();
        yield return new WaitForSeconds(lookDuration);
        FaceRight();
        yield return new WaitForSeconds(lookDuration * 0.6f);

        yield return new WaitForSeconds(messageDelay);
        yield return StartCoroutine(ShowMessage());
        yield return new WaitForSeconds(2.5f);
        yield return StartCoroutine(HideMessage());
        transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        yield return new WaitForSeconds(0.3f);

        BossIntroCutscene boss = FindFirstObjectByType<BossIntroCutscene>();
        if (boss != null)
        {
            boss.StartBossIntro();
        }
        else
        {
            DisablePlayerControl(false);
        }
    }

    void FaceRight() { if (sr != null) sr.flipX = false; }
    void FaceLeft() { if (sr != null) sr.flipX = true; }

    IEnumerator LandingSquish()
    {
        float t = 0f, duration = 0.25f;
        Vector3 original = Vector3.one;
        Vector3 squished = new Vector3(1.3f, 0.7f, 1f);

        while (t < duration)
        {
            t += Time.deltaTime;
            float ratio = t / duration;
            transform.localScale = ratio < 0.5f
                ? Vector3.Lerp(original, squished, ratio * 2f)
                : Vector3.Lerp(squished, original, (ratio - 0.5f) * 2f);
            yield return null;
        }
        transform.localScale = original;
    }

    IEnumerator ShowMessage()
    {
            if (messagePanel == null)
            if (messagePanel == null)
            {
                GameObject bgCanvas = GameObject.Find("BackGroundCanvas");
                if (bgCanvas != null)
                {
                    Transform found = bgCanvas.transform.Find("MessagePanel");
                    if (found != null)
                        messagePanel = found.gameObject;
                }
            }


            if (messagePanel == null)
            {
                yield break;
            }

            messagePanel.SetActive(true);


        RectTransform rt = messagePanel.GetComponent<RectTransform>();
        if (rt != null)
        {
            Vector2 shownPos = rt.anchoredPosition;
            Vector2 hiddenPos = shownPos + new Vector2(0, 80f);
            rt.anchoredPosition = hiddenPos;

            float t = 0f, dur = 0.35f;
            while (t < dur)
            {
                t += Time.deltaTime;
                float eased = 1f - Mathf.Pow(1f - (t / dur), 3f);
                rt.anchoredPosition = Vector2.Lerp(hiddenPos, shownPos, eased);
                yield return null;
            }
            rt.anchoredPosition = shownPos;
        }
    }

    IEnumerator HideMessage()
    {
        if (messagePanel == null) yield break;

        RectTransform rt = messagePanel.GetComponent<RectTransform>();
        if (rt != null)
        {
            Vector2 shownPos = rt.anchoredPosition;
            Vector2 hiddenPos = shownPos + new Vector2(0, 80f);

            float t = 0f, dur = 0.3f;
            while (t < dur)
            {
                t += Time.deltaTime;
                float ratio = t / dur;
                rt.anchoredPosition = Vector2.Lerp(shownPos, hiddenPos, ratio * ratio);
                yield return null;
            }
        }
        messagePanel.SetActive(false);
    }

    void DisablePlayerControl(bool disabled)
    {
        if (rb != null)
        {
            rb.bodyType = disabled ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
            rb.linearVelocity = Vector2.zero;
        }

        var controller = GetComponent<PlayerMovement>();
        if (controller != null) controller.enabled = !disabled;
    }
}