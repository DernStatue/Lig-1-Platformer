
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlatformerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float acceleration = 60f;
    public float airControl = 0.5f;
    public float turnSpeed = 10f;

    [Header("Sprint")]
    public float sprintMultiplier = 1.6f;

    [Header("Jump")]
    public float jumpHeight = 2.2f;
    public float gravity = -25f;
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundMask;

    [Header("Platform Riding")]
    public float platformRayDistance = 1.2f;

    private CharacterController controller;
    private Vector3 velocity;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool isGrounded;
    private bool isSprinting;

    // Public read-only state for animation and other scripts
    public bool IsGrounded => isGrounded;
    public bool IsSprinting => isSprinting;
    public float VerticalVelocity => velocity.y;
    public Vector3 HorizontalVelocity =>
        new Vector3(velocity.x, 0f, velocity.z);
    public float HorizontalSpeed => HorizontalVelocity.magnitude;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        CheckPlatform();
        CheckGround();
        HandleJump();
        HandleMovement();

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void CheckGround()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(
                groundCheck.position,
                groundCheckRadius,
                groundMask
            );
        }
        else
        {
            // Fallback if Ground Check hasn't been assigned.
            isGrounded = controller.isGrounded;
        }

        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        coyoteTimer = isGrounded
            ? coyoteTime
            : coyoteTimer - Time.deltaTime;
    }

    void HandleJump()
    {
        jumpBufferTimer = Input.GetButtonDown("Jump")
            ? jumpBufferTime
            : jumpBufferTimer - Time.deltaTime;

        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }
    }

    void HandleMovement()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift) && isGrounded;

        float currentMoveSpeed =
            moveSpeed * (isSprinting ? sprintMultiplier : 1f);

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Transform cam = Camera.main != null
            ? Camera.main.transform
            : transform;

        Vector3 camForward = cam.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cam.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 moveDir =
            (camForward * v + camRight * h).normalized;

        Vector3 targetVel = moveDir * currentMoveSpeed;
        float control = isGrounded ? 1f : airControl;

        velocity.x = Mathf.Lerp(
            velocity.x,
            targetVel.x,
            acceleration * control * Time.deltaTime
        );

        velocity.z = Mathf.Lerp(
            velocity.z,
            targetVel.z,
            acceleration * control * Time.deltaTime
        );

        if (moveDir.sqrMagnitude > 0.01f)
        {
            transform.forward = Vector3.Slerp(
                transform.forward,
                moveDir,
                turnSpeed * Time.deltaTime
            );
        }
    }

    void CheckPlatform()
    {
        if (Physics.Raycast(
            transform.position,
            Vector3.down,
            out RaycastHit hit,
            platformRayDistance,
            groundMask))
        {
            Transform hitPlatform = hit.collider.transform;

            var moving = hitPlatform.GetComponent<MovingPlatform>();
            var rotating = hitPlatform.GetComponent<RotatingPlatform>();

            if (moving != null)
            {
                controller.Move(moving.LastDelta);
                ApplyRotationDelta(
                    moving.LastRotationDelta,
                    hitPlatform
                );
            }
            else if (rotating != null)
            {
                ApplyRotationDelta(
                    rotating.LastRotationDelta,
                    hitPlatform
                );
            }
        }
    }

    void ApplyRotationDelta(Quaternion delta, Transform platform)
    {
        float yawDelta = Mathf.DeltaAngle(
            0f,
            delta.eulerAngles.y
        );

        transform.Rotate(0f, yawDelta, 0f);

        Vector3 toPlayer = transform.position - platform.position;
        Vector3 rotated = Quaternion.Euler(0f, yawDelta, 0f)
            * toPlayer;

        controller.Move(rotated - toPlayer);
    }

    // Called by BouncePad or other external triggers.
    public void SetVerticalVelocity(float v)
    {
        velocity.y = v;
    }
}