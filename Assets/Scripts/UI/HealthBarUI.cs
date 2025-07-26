using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HealthBarUI : MonoBehaviour
{
    public Image fillImage;
    public PlayerHealth playerHealth;
    public float smoothSpeed = 5f;

    private float targetFill;

    public TMP_Text healthText;

    private float blinkTimer = 0f;
    private bool blinkOn = true;



    void Update()
    {
        float healthPercent = (float)playerHealth.CurrentHealth / playerHealth.MaxHealth;
        targetFill = Mathf.Clamp01(healthPercent);

        // Smooth Transition
        fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, targetFill, Time.deltaTime * smoothSpeed);

        // Balken-Farbe aktualisieren
        if (targetFill > 0.5f)
        {
            fillImage.color = Color.Lerp(Color.yellow, Color.green, (targetFill - 0.5f) * 2f);
        }
        else
        {
            fillImage.color = Color.Lerp(Color.red, Color.yellow, targetFill * 2f);
        }

        // Text aktualisieren
        healthText.text = $"{playerHealth.CurrentHealth}";

        // Blinken bei niedriger Gesundheit
        if (healthPercent <= 0.25f)
        {
            blinkTimer += Time.deltaTime;

            if (blinkTimer >= 0.5f)
            {
                blinkTimer = 0f;
                blinkOn = !blinkOn;
            }

            healthText.color = blinkOn ? Color.red : Color.white;
        }
        else
        {
            healthText.color = Color.white;
            blinkTimer = 0f;
            blinkOn = true;
        }
    }

}
