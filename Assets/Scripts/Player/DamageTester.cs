using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class DamageTester : MonoBehaviour
{
    public PlayerHealth playerHealth;

    private PlayerControls input;

    void Awake()
    {
        input = new PlayerControls();
    }

    void OnEnable()
    {
        input.Enable();
        input.Player.TestDamage.performed += OnTestDamage;
    }

    void OnDisable()
    {
        input.Player.TestDamage.performed -= OnTestDamage;
        input.Disable();
    }

    void OnTestDamage(InputAction.CallbackContext context)
    {
        playerHealth.TakeDamageServerRpc(25, (int)playerHealth.team.Value);
    }
}
