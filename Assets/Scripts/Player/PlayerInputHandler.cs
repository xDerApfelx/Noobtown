using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float walkingSpeed = 5f;
    public float jumpHeight = 2f;

    [Header("Look")]
    public float mouseSensitivity = 3f;
    public Transform cameraPivot;

    [Header("Gravity")]
    public float gravity = -9.81f;

    private CharacterController cc;
    private PlayerControls input;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalSpeed;
    private float pitch;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        input = new PlayerControls();

        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += _ => moveInput = Vector2.zero;

        input.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        input.Player.Look.canceled += _ => lookInput = Vector2.zero;

        input.Player.Jump.performed += _ => TryJump();
    }

    void OnEnable()
    {
        input.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDisable()
    {
        input.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        HandleLook();
        HandleMovement();
    }

    void HandleLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        if (cc.isGrounded && verticalSpeed < 0f)
            verticalSpeed = -2f;      // kleiner Down-Kick hält den Controller grounded

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move *= walkingSpeed;

        verticalSpeed += gravity * Time.deltaTime;
        move.y = verticalSpeed;

        cc.Move(move * Time.deltaTime);
    }

    void TryJump()
    {
        if (cc.isGrounded)
            verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
}
