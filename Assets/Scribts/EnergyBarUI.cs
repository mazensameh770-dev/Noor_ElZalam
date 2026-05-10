using UnityEngine;
using UnityEngine.UI;

public class EnergyBarUI : MonoBehaviour
{
    public Image segment1;
    public Image segment2;
    public Image segment3;

    public Color emptyColor = Color.gray;
    public Color filledColor = Color.cyan;


    void Update()
    {
        int stars = GameManager.Instance.currentStars;

        segment1.color = stars >= 3 ? filledColor : emptyColor;

        segment2.color = stars >= 6 ? filledColor : emptyColor;

        segment3.color = stars >= 9 ? filledColor : emptyColor;
    }
}