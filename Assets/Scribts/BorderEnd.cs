using UnityEngine;

public class BorderEnd : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            BorderMessage bm = FindFirstObjectByType<BorderMessage>();
            if (bm != null) bm.ShowBorderEndMessage();
        }
    }
}