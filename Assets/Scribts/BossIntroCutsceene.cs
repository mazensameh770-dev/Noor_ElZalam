using System.Collections;
using UnityEngine;
using TMPro;

public class BossIntroCutscene : MonoBehaviour
{
    [Header("Camera Shake")]
    public Camera mainCamera;
    public float shakeDuration = 1.5f;
    public float shakeMagnitude = 0.3f;

    [Header("Boss Warning Message")]
    public GameObject bossMessagePanel;
    public TextMeshProUGUI bossMessageText;
    public string bossWarningText = "stand ready for my arrival, worm";

    [Header("Boss Arrival Message")]
    public GameObject arrivalMessagePanel;
    public TextMeshProUGUI arrivalMessageText;
    public string arrivalText = "Let the final battle between darkness and light begin";

    [Header("Boss")]
    public GameObject bossObject;
    public float bossStartY = 10f;   
    public float bossEndY = -2.5f;   
    public float bossFallDuration = 1.2f; 
    private Animator bossAnimator;
    private SpriteRenderer bossSR;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip earthquakeSound;
    public AudioClip bossVoice1;   
    public AudioClip bossVoice2;   

    [Header("Game Start")]
    public GameManager gameManager;

    void StartGame()
    {
        if (gameManager != null)
            gameManager.StartGame();

        IntroCutscene intro = FindFirstObjectByType<IntroCutscene>();
        if(intro != null && intro.firepoint != null)
        {
            intro.firepoint.SetActive(true);
        }

        BossController boss = bossObject.GetComponent<BossController>();
        if (boss != null)
            boss.StartBossFight();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.bodyType = RigidbodyType2D.Dynamic;

            var controller = player.GetComponent<PlayerMovement>();
            if (controller != null)
                controller.enabled = true;
        }
    }
    public void StartBossIntro()
    {
        StartCoroutine(BossIntroSequence());
    }

    IEnumerator BossIntroSequence()
    {
        yield return StartCoroutine(Phase1_WarningMessage());

        yield return StartCoroutine(Phase2_BossFallsDown());

        yield return StartCoroutine(Phase3_LandingAndFinalMessage());
    }

    IEnumerator Phase1_WarningMessage()
    {
        yield return StartCoroutine(ShowPanel(bossMessagePanel, bossMessageText, bossWarningText));

        if (audioSource != null && bossVoice1 != null)
            audioSource.PlayOneShot(bossVoice1);

        float waitTime = bossVoice1 != null ? bossVoice1.length : 2f;
        yield return new WaitForSeconds(waitTime);

        yield return StartCoroutine(HidePanel(bossMessagePanel));
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator Phase2_BossFallsDown()
    {
        if (bossObject == null) yield break;

        bossObject.SetActive(true);
        bossAnimator = bossObject.GetComponent<Animator>();
        bossSR = bossObject.GetComponent<SpriteRenderer>();

        if (bossSR != null) bossSR.flipX = true;

        Vector3 startPos = bossObject.transform.position;
        startPos.y = bossStartY;
        bossObject.transform.position = startPos;

        Vector3 endPos = new Vector3(startPos.x, bossEndY, startPos.z);

        if (bossAnimator != null)
            bossAnimator.SetBool("isFlying", true);

        float elapsed = 0f;
        while (elapsed < bossFallDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bossFallDuration;
            float easedT = t * t; 
            bossObject.transform.position = Vector3.Lerp(startPos, endPos, easedT);
            yield return null;
        }

        bossObject.transform.position = endPos;

        if (bossAnimator != null)
        {
            bossAnimator.SetBool("isFlying", false);
        }
    }

    IEnumerator Phase3_LandingAndFinalMessage()
    {
        if (audioSource != null && earthquakeSound != null)
            audioSource.PlayOneShot(earthquakeSound);

        yield return StartCoroutine(ShakeCamera(shakeDuration, shakeMagnitude));

        yield return new WaitForSeconds(0.4f);

        yield return StartCoroutine(ShowPanel(arrivalMessagePanel, arrivalMessageText, arrivalText));

        if (audioSource != null && bossVoice2 != null)
            audioSource.PlayOneShot(bossVoice2);

        float waitTime = bossVoice2 != null ? bossVoice2.length : 3f;
        yield return new WaitForSeconds(waitTime);

        yield return StartCoroutine(HidePanel(arrivalMessagePanel));

        yield return new WaitForSeconds(0.5f);

        StartGame();
    }

    IEnumerator ShakeCamera(float duration, float magnitude)
    {
        if (mainCamera == null) yield break;

        Vector3 originalPos = mainCamera.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float xOffset = Random.Range(-1f, 1f) * magnitude;
            float yOffset = Random.Range(-1f, 1f) * magnitude;
            mainCamera.transform.position = new Vector3(
                originalPos.x + xOffset,
                originalPos.y + yOffset,
                originalPos.z
            );
            yield return null;
        }

        mainCamera.transform.position = originalPos;
    }

    IEnumerator ShowPanel(GameObject panel, TextMeshProUGUI textComp, string message)
    {
        if (panel == null) yield break;
        if (textComp != null) textComp.text = message;
        panel.SetActive(true);

        RectTransform rt = panel.GetComponent<RectTransform>();
        if (rt != null)
        {
            Vector2 shownPos = rt.anchoredPosition;
            Vector2 hiddenPos = shownPos + new Vector2(0, 100f);
            rt.anchoredPosition = hiddenPos;

            float t = 0f, dur = 0.4f;
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

    IEnumerator HidePanel(GameObject panel)
    {
        if (panel == null) yield break;

        RectTransform rt = panel.GetComponent<RectTransform>();
        if (rt != null)
        {
            Vector2 shownPos = rt.anchoredPosition;
            Vector2 hiddenPos = shownPos + new Vector2(0, 100f);

            float t = 0f, dur = 0.3f;
            while (t < dur)
            {
                t += Time.deltaTime;
                float ratio = t / dur;
                rt.anchoredPosition = Vector2.Lerp(shownPos, hiddenPos, ratio * ratio);
                yield return null;
            }
        }
        panel.SetActive(false);
    }
}