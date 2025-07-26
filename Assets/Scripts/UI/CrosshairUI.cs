using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    [Header("Lines")]
    public RectTransform lineUp;
    public RectTransform lineDown;
    public RectTransform lineLeft;
    public RectTransform lineRight;

    [Header("Settings")]
    public float smoothSpeed = 10f;          // Wie schnell das UI die Zielgröße erreicht
    public float baseGap = 8f;   // 8  Pixel beim Stehen
    public float maxGap = 40f;  // 40 Pixel bei größter Ungenauigkeit
                                // größter möglicher Abstand

    float currentGap;                        // Abstand der gerade angezeigt wird
    float targetGap;                         // Abstand den wir erreichen wollen

    void Update()
    {
        currentGap = Mathf.Lerp(currentGap, targetGap, Time.deltaTime * smoothSpeed);
        ApplyGap(currentGap);
    }

    void ApplyGap(float gap)
    {
        lineUp.anchoredPosition = new Vector2(0, gap);
        lineDown.anchoredPosition = new Vector2(0, -gap);
        lineLeft.anchoredPosition = new Vector2(-gap, 0);
        lineRight.anchoredPosition = new Vector2(gap, 0);
    }


    // Aufruf durch deine Waffen-Logik
    public void SetSpreadPercent(float percent)
    {
        targetGap = Mathf.Lerp(baseGap, maxGap, percent);
    }
}
