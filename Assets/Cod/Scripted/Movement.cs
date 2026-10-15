
using UnityEngine;

[DefaultExecutionOrder(0)]
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
    private bool jumpedThisFrame;

    private Transform currentPlatform;
    private MovingPlatform currentMovingPlatform;
    private RotatingPlatform currentRotatingPlatform;

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
        jumpedThisFrame = false;

        CheckGround();
        HandleJump();

        if (!jumpedThisFrame)
            CheckPlatform();

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
                groundMask,
                QueryTriggerInteraction.Ignore
            );
        }
        else
        {
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
            jumpedThisFrame = true;
            ClearPlatform();

            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            isGrounded = false;
        }
    }

    void HandleMovement()
    {
        isSprinting =
            Input.GetKey(KeyCode.LeftShift) && isGrounded;

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
        // Apply movement from the platform detected last frame.
        if (currentPlatform != null)
        {
            Vector3 platformDelta = Vector3.zero;
            Quaternion rotationDelta = Quaternion.identity;

            if (currentMovingPlatform != null)
            {
                platformDelta = currentMovingPlatform.LastDelta;
                rotationDelta =
                    currentMovingPlatform.LastRotationDelta;
            }
            else if (currentRotatingPlatform != null)
            {
                platformDelta = currentRotatingPlatform.LastDelta;
                rotationDelta =
                    currentRotatingPlatform.LastRotationDelta;
            }

            Vector3 previousPlatformPosition =
                currentPlatform.position - platformDelta;

            Vector3 oldPlayerPosition = transform.position;

            Vector3 newPlayerPosition =
                currentPlatform.position +
                rotationDelta *
                (oldPlayerPosition - previousPlatformPosition);

            controller.Move(newPlayerPosition - oldPlayerPosition);

            // Carry the player's facing direction with platform yaw.
            float yawDelta = Mathf.DeltaAngle(
                0f,
                rotationDelta.eulerAngles.y
            );

            transform.Rotate(0f, yawDelta, 0f);
        }

        // Find a supported platform beneath the player's feet.
        Vector3 origin = groundCheck != null
            ? groundCheck.position + Vector3.up * 0.1f
            : transform.position;

        if (Physics.Raycast(
            origin,
            Vector3.down,
            out RaycastHit hit,
            platformRayDistance,
            groundMask,
            QueryTriggerInteraction.Ignore))
        {
            MovingPlatform moving =
                hit.collider.GetComponentInParent<MovingPlatform>();

            RotatingPlatform rotating =
                hit.collider.GetComponentInParent<RotatingPlatform>();

            if (moving != null)
            {
                SetPlatform(moving.transform, moving, null);
                return;
            }

            if (rotating != null)
            {
                SetPlatform(rotating.transform, null, rotating);
                return;
            }
        }

        ClearPlatform();
    }

    void SetPlatform(
        Transform platform,
        MovingPlatform moving,
        RotatingPlatform rotating)
    {
        currentPlatform = platform;
        currentMovingPlatform = moving;
        currentRotatingPlatform = rotating;
    }

    void ClearPlatform()
    {
        currentPlatform = null;
        currentMovingPlatform = null;
        currentRotatingPlatform = null;
    }

    public void SetVerticalVelocity(float value)
    {
        ClearPlatform();
        velocity.y = value;
    }
}
