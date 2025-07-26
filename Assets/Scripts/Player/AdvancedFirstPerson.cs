using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class AdvancedFirstPerson : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    private Vector3 horizontalVelocity;




    public float sprintSpeed = 8f;
    public float jumpHeight = 2f;
    [Range(0f, 1f)] public float airControlFactor = 0.5f;

    [Header("Look")]
    public float mouseSensitivity = 3f;
    public float lookSmooth = 0.15f;
    public Transform cameraPivot;

    [Header("Gravity")]
    public float gravity = -9.81f;

    private CharacterController cc;
    private PlayerControls input;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalSpeed;
    private float pitch;
    private Vector2 currentLook;
    private Vector2 lookVelocity;
    private Vector3 groundNormal = Vector3.up;

    public bool IsGrounded { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsJumping { get; private set; }

    public bool IsCrouching { get; private set; }
    public float MoveSpeed { get; private set; }   // nur lesen




    float jumpFlagTimer;                       // läuft nach jedem Absprung kurz



    void TryJump()
    {
        if (cc.isGrounded)
        {
            verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpFlagTimer = 1f;              // 0,2 s lang gilt „springt gerade“
        }
    }


    void Awake()
    {
        cc = GetComponent<CharacterController>();
        input = new PlayerControls();

        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += _ => moveInput = Vector2.zero;

        input.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        input.Player.Look.canceled += _ => lookInput = Vector2.zero;

        input.Player.Jump.performed += _ => TryJump();

        // crouch-Input (neu)
        input.Player.Crouch.performed += _ => IsCrouching = true;
        input.Player.Crouch.canceled += _ => IsCrouching = false;
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
        if (!cc.enabled) return;

        /* ---------- Look & Move ---------- */
        HandleLook();
        HandleMovement();
        HandleSlopeSlide();

        /* ---------- Status setzen ---------- */
        IsGrounded = cc.isGrounded;
        IsSprinting = input.Player.Sprint.IsPressed();

        // Jump-Flag läuft noch? Dann bleibt IsJumping true
        if (jumpFlagTimer > 0f)
        {
            jumpFlagTimer -= Time.deltaTime;
            IsJumping = true;
        }
        else
        {
            IsJumping = false;
        }
    }


    void HandleLook()
    {
        Vector2 targetLook = lookInput * mouseSensitivity;
        currentLook = Vector2.SmoothDamp(currentLook, targetLook, ref lookVelocity, lookSmooth);

        pitch -= currentLook.y;
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        transform.Rotate(Vector3.up * currentLook.x);
    }

    void HandleMovement()
    {
        bool grounded = cc.isGrounded;

        if (grounded && verticalSpeed < 0f)
            verticalSpeed = -2f;

        float baseSpeed = input.Player.Sprint.IsPressed() ? sprintSpeed : walkSpeed;

        // Eingabe in Welt­richtung umrechnen
        Vector3 wishMove = transform.right * moveInput.x + transform.forward * moveInput.y;
        wishMove *= baseSpeed;

        if (grounded)
        {
            horizontalVelocity = wishMove;                    // volle Kontrolle
        }
        else
        {
            // In der Luft nur seitliches Lenken dämpfen
            Vector3 lateral = Vector3.ProjectOnPlane(wishMove, transform.forward);
            Vector3 forward = Vector3.Project(wishMove, transform.forward);

            forward *= 1f;                                   // voller Vorwärts-Schub
            lateral *= airControlFactor;                     // gedämpfte Seitenkontrolle

            horizontalVelocity = forward + lateral;
        }

        verticalSpeed += gravity * Time.deltaTime;

        Vector3 move = horizontalVelocity;
        move.y = verticalSpeed;

        cc.Move(move * Time.deltaTime);

        MoveSpeed = new Vector3(cc.velocity.x, 0, cc.velocity.z).magnitude;

    }

    void HandleSlopeSlide()
    {
        if (!cc.isGrounded) return;

        float slopeAngle = Vector3.Angle(Vector3.up, groundNormal);
        if (slopeAngle > cc.slopeLimit)
        {
            Vector3 slideDir = new Vector3(groundNormal.x, -groundNormal.y, groundNormal.z);
            cc.Move(slideDir * Time.deltaTime);
        }
    }

    // Hier holen wir uns die Boden‐Normale
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (Vector3.Angle(hit.normal, Vector3.up) < 89f)
            groundNormal = hit.normal;
    }
}
