using UnityEngine;

[CreateAssetMenu(menuName = "Shooter/Weapon Profile")]
public class WeaponProfile : ScriptableObject
{
    /* - Allgemein */
    public Sprite crosshairSprite;

    [Header("Treffergenauigkeit")]
    [Range(0f, 1f)] public float baseSpread = 0.10f;
    [Range(0f, 1f)] public float walkSpread = 0.15f;
    [Range(0f, 1f)] public float hipSpread = 0.40f;
    [Range(0f, 1f)] public float runSpread = 0.60f;
    [Range(0f, 1f)] public float crouchSpread = 0.05f;
    [Range(0f, 1f)] public float jumpSpread = 0.90f;
    [Range(0f, 1f)] public float adsSpread = 0.02f;

    /* - Schusswaffen */
    [Header("Schussparameter")]
    public bool isAutomatic = false;     // Einzel- oder Dauerfeuer
    public float fireRate = 4f;           // Schüsse pro Sekunde
    public int pellets = 1;             // Shotgun setzt hier >1
    public float damage = 20f;
    public float range = 100f;            // Reichweite für Raycast

    [Header("Magazine & Ammo")]
    public int magazineSize = 12;
    public int maxAmmo = 120;


    [Header("Rückstoß")]
    public float recoilKickback = 0.05f;   // Rückstoß auf Kamera
    public float recoilRecovery = 10f;     // Wie schnell zurück zur Mitte

    /* - Nahkampf */
    [Header("Nahkampf")]
    public bool isMelee = false;         // Faust oder Messer
    public float meleeRange = 2f;
    public float meleeDamage = 35f;


    [Header("FX / Audio")]
    public GameObject muzzleFlashPrefab;
    public AudioClip shotSfx;
    public AudioClip reloadSfx;


}
