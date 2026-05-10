using System.Collections;
using UnityEngine;
using TMPro;

public class BorderMessage : MonoBehaviour
{
    [Header("Messages")]
    public string borderEndMessage = "There's a border here, don't try";
    public string borderStartMessage = "You need to collect all the stars to remove the border.";
    public string allStarsMessage = "The border has been removed, let's move on to the next level.";
    public string allStarsMessageLevel3 = "Now go down into the small hole after the demolition, not the big one 😆";

    [Header("UI")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;

    [Header("Audio")]
    public AudioClip messageSound;

    private static BorderMessage instance;

    void Awake()
    {
        instance = this;
    }

    public void ShowBorderEndMessage()
    {
        StartCoroutine(ShowMessage(borderEndMessage));
    }

    public void ShowBorderStartMessage()
    {
        StartCoroutine(ShowMessage(borderStartMessage));
    }

    public static void ShowAllStarsMessage()
    {
        if (instance == null) return;

        int currentScene = UnityEngine.SceneManagement.SceneManager
                           .GetActiveScene().buildIndex;


        if (currentScene == 2)
        {
            instance.StartCoroutine(
                instance.ShowMessage(instance.allStarsMessageLevel3));
        }
        else
        {
            instance.StartCoroutine(
                instance.ShowMessage(instance.allStarsMessage));
        }
    }

    IEnumerator ShowMessage(string msg)
    {
        if (messagePanel == null || messageText == null) yield break;

        messageText.text = msg;
        messagePanel.SetActive(true);

        if (messageSound != null)
            AudioSource.PlayClipAtPoint(messageSound, Camera.main.transform.position);

        yield return new WaitForSeconds(3f);

        messagePanel.SetActive(false);
    }
}