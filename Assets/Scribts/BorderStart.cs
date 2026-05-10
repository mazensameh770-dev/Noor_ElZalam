using UnityEngine;

public class BorderStart : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            BorderMessage bm = FindFirstObjectByType<BorderMessage>();
            if (bm != null) bm.ShowBorderStartMessage();
        }
    }
}