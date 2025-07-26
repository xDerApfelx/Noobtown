using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 200;
    private float currentHealth;

    public GameObject deathMessageUI; // UI-Canvas oder Text
    public Transform respawnPoint;    // Punkt zum Respawnen
    public CharacterController controller;

    public float regenDelay = 5f;   // warten bis Regeneration beginnt
    public float regenRate = 10f;  // Start-HP pro Sekunde
    public float rampInterval = 5f;   // alle 5 s ankurbeln
    public float rampMultiplier = 1.5f; // x-Faktor pro Stufe

    private float timeSinceLastDamage;
    private float timeInRegen;             // läuft nur während Heilung
    private float currentRegenRate;        // steigt stufenweise


    private void Start()
    {
        currentHealth = maxHealth;
        deathMessageUI.SetActive(false);
        currentRegenRate = regenRate;


    }

    public int CurrentHealth => Mathf.RoundToInt(currentHealth);   // 2) Property wandelt fürs UI um
    public int MaxHealth => maxHealth;


    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        timeSinceLastDamage = 0f;   // ← reicht hier

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }

        timeInRegen = 0f;
        currentRegenRate = regenRate;   // Basis
    }


    void Die()
    {
        // Bewegung deaktivieren
        controller.enabled = false;

        // Meldung anzeigen
        deathMessageUI.SetActive(true);

        // Respawn einleiten
        StartCoroutine(RespawnAfterDelay(2f));
    }

    IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        currentHealth = maxHealth;
        timeSinceLastDamage = 0f;
        timeInRegen = 0f;
        currentRegenRate = regenRate;

        controller.enabled = false;
        transform.position = respawnPoint.position;
        controller.enabled = true;

        deathMessageUI.SetActive(false);
    }


    void Update()
    {
        if (currentHealth <= 0) return;            // tot

        timeSinceLastDamage += Time.deltaTime;

        // ► Regeneration aktiv?
        if (currentHealth < maxHealth && timeSinceLastDamage >= regenDelay)
        {
            // 1) Timer für Ramp-Logik
            timeInRegen += Time.deltaTime;

            // 2) Alle rampInterval-Sekunden Tempo erhöhen
            if (timeInRegen >= rampInterval)
            {
                timeInRegen = 0f;
                currentRegenRate *= rampMultiplier;   // z. B. 10 → 15 → 22.5 …
            }

            // 3) Heilen
            currentHealth += currentRegenRate * Time.deltaTime;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }
    }

}
