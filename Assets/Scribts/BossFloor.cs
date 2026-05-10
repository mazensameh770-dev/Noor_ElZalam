using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class BossFloor : MonoBehaviour
{
    public int starsRequired = 9;
    private bool isDestroyed = false;
    public AudioSource audioSource;
    public AudioClip earthquakeSound;

    void Update()
    {
        if (!isDestroyed && GameManager.Instance.currentStars >= starsRequired)
        {
            isDestroyed = true;
            StartCoroutine(ShakeAndDestroy());
        }
    }

    IEnumerator ShakeAndDestroy()
    {
        if (audioSource != null && earthquakeSound != null)
            audioSource.PlayOneShot(earthquakeSound);
        Vector3 originalPos = transform.position;
        float elapsed = 0f;
        float duration = 2f;
        float magnitude = 0.2f;

        while (elapsed < duration)
        {
            float x = originalPos.x + Random.Range(-1f, 1f) * magnitude;
            float y = originalPos.y + Random.Range(-1f, 1f) * magnitude;
            transform.position = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}