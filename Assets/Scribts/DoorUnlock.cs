using UnityEngine;

public class DoorUnlock : MonoBehaviour
{
    public int requiredStars = 3;
    private bool opened = false;

    void Update()
    {
        if (!opened && GameManager.Instance.HasEnoughStars(requiredStars))
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        opened = true;
        gameObject.SetActive(false); 
    }
}