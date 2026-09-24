using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlatformerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float acceleration = 60f;
    public float airControl = 0.5f;

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
    private Transform currentPlatform;

    void Start()
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
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // small stick-to-ground force

        coyoteTimer = isGrounded ? coyoteTime : coyoteTimer - Time.deltaTime;
    }

    void HandleJump()
    {
        jumpBufferTimer = Input.GetButtonDown("Jump") ? jumpBufferTime : jumpBufferTimer - Time.deltaTime;

        if (jumpBufferTimer > 0 && coyoteTimer > 0)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpBufferTimer = 0;
            coyoteTimer = 0;
        }
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 camForward = Camera.main.transform.forward; camForward.y = 0; camForward.Normalize();
        Vector3 camRight = Camera.main.transform.right; camRight.y = 0; camRight.Normalize();
        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        Vector3 targetVel = moveDir * moveSpeed;
        float control = isGrounded ? 1f : airControl;
        velocity.x = Mathf.Lerp(velocity.x, targetVel.x, acceleration * control * Time.deltaTime);
        velocity.z = Mathf.Lerp(velocity.z, targetVel.z, acceleration * control * Time.deltaTime);

        if (moveDir.sqrMagnitude > 0.01f)
            transform.forward = Vector3.Slerp(transform.forward, moveDir, 10f * Time.deltaTime);
    }

    void CheckPlatform()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, platformRayDistance, groundMask))
        {
            Transform hitPlatform = hit.collider.transform;
            var moving = hitPlatform.GetComponent<MovingPlatform>();
            var rotating = hitPlatform.GetComponent<RotatingPlatform>();

            if (moving != null)
            {
                controller.Move(moving.LastDelta);
                ApplyRotationDelta(moving.LastRotationDelta, hitPlatform);
                currentPlatform = hitPlatform;
            }
            else if (rotating != null)
            {
                ApplyRotationDelta(rotating.LastRotationDelta, hitPlatform);
                currentPlatform = hitPlatform;
            }
            else
            {
                currentPlatform = null;
            }
        }
        else
        {
            currentPlatform = null;
        }
    }

    void ApplyRotationDelta(Quaternion delta, Transform platform)
    {
        Vector3 eulerDelta = delta.eulerAngles;
        // Normalize angle to avoid huge jumps from 359 -> 0 wraparound
        float yawDelta = Mathf.DeltaAngle(0f, eulerDelta.y);

        transform.Rotate(0f, yawDelta, 0f);

        Vector3 toPlayer = transform.position - platform.position;
        Vector3 rotated = Quaternion.Euler(0f, yawDelta, 0f) * toPlayer;
        controller.Move(rotated - toPlayer);
    }

    // Called by BouncePad or similar external triggers
    public void SetVerticalVelocity(float v)
    {
        velocity.y = v;
    }
}