using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;   // <– neues Input-System
using Unity.Netcode;

public class WeaponController : MonoBehaviour
{
    public CrosshairUI crosshair;

    public AdvancedFirstPerson player;   // im Inspector zuweisen

    public WeaponProfile[] profiles;          // Fäuste = 0, Pistole = 1

    int current;
    PlayerControls input;                     // auto-generierte Klasse
    float nextShotTime;          // Cooldown‐Timer
    int currentAmmo;           // Restkugeln im Magazin
    public LayerMask hitMask;     // im Inspector setzen: alles außer „Player“
    private PlayerHealth shooterHealth;


    void Awake()
    {
        input = new PlayerControls();
    }

    void OnEnable()
    {
        input.Enable();
        input.Player.SwitchWeapon.performed += _ => ToggleWeapon();
        input.Player.Fire.performed += _ => TryFire();

    }

    void OnDisable()
    {
        input.Player.SwitchWeapon.performed -= _ => ToggleWeapon();
        input.Disable();
        input.Player.Fire.performed -= _ => TryFire();

    }

    void Start()
    {
        if (profiles == null || profiles.Length == 0)
        {
            Debug.LogError("WeaponController: Kein WeaponProfile zugewiesen!");
            enabled = false;                      // Script anhalten
            return;
        }
        current = 0;
        ApplyProfile(profiles[current]);

        shooterHealth = GetComponentInParent<PlayerHealth>();

        currentAmmo = profiles[current].magazineSize;

    }

    void Update()
    {
        var p = profiles[current];
        float spread;

        if (!player.IsGrounded && player.IsJumping) spread = p.jumpSpread;
        else if (player.IsCrouching) spread = p.crouchSpread;
        else if (player.IsSprinting) spread = p.runSpread;
        else if (player.MoveSpeed > 0.1f) spread = p.walkSpread;     // normales Gehen
        else spread = p.baseSpread;     // ruhig stehen

        crosshair.SetSpreadPercent(spread);
    }




    /* ---------- helpers ---------- */

    void ToggleWeapon()                 // Q gedrückt
    {
        current = (current + 1) % profiles.Length;   // wechselt 0 ↔ 1 ↔ …
        ApplyProfile(profiles[current]);
    }

    void ApplyProfile(WeaponProfile p)
    {
        Sprite s = p.crosshairSprite;

        foreach (var img in new[] {
             crosshair.lineUp, crosshair.lineDown,
             crosshair.lineLeft, crosshair.lineRight })
        {
            // —— einfach nur Sprite setzen, NICHT aktiv/deaktivieren
            if (s) img.GetComponent<UnityEngine.UI.Image>().sprite = s;
        }
    }


    void TryFire()
    {
        var p = profiles[current];

        // 1) Cooldown prüfen
        if (Time.time < nextShotTime) return;

        // 2) Munition oder Nahkampf?
        if (p.isMelee)
        {
            MeleeAttack(p);
            nextShotTime = Time.time + 1f / p.fireRate;     // fireRate auch für Faust
            return;
        }

        if (currentAmmo <= 0) { Debug.Log("Leer – später reload"); return; }

        currentAmmo--;
        ShootRay(p);

        nextShotTime = Time.time + 1f / p.fireRate;
    }

    void ShootRay(WeaponProfile p)
    {
        Vector3 origin = Camera.main.transform.position + Camera.main.transform.forward * 0.1f;

        Vector3 dir = Camera.main.transform.forward;

        // trifft nur Schichten in hitMask, ignoriert Trigger
        if (Physics.Raycast(origin, dir, out RaycastHit hit,
                            p.range, hitMask, QueryTriggerInteraction.Ignore))
        {
            PlayerHealth hp = hit.collider.GetComponent<PlayerHealth>();
            if (hp)
            {
                hp.TakeDamageServerRpc((int)p.damage, (int)shooterHealth.team.Value);
            }
        }
    }

    void MeleeAttack(WeaponProfile p)
    {
        Vector3 origin = Camera.main.transform.position;
        Vector3 dir = Camera.main.transform.forward;

        if (Physics.Raycast(origin, dir, out RaycastHit hit,
                            p.meleeRange, hitMask, QueryTriggerInteraction.Ignore))
        {
            PlayerHealth hp = hit.collider.GetComponent<PlayerHealth>();
            if (hp)
            {
                hp.TakeDamageServerRpc((int)p.meleeDamage, (int)shooterHealth.team.Value);
            }
        }
    }


}
