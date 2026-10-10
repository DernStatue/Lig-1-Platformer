using UnityEngine;

public class AnimatorBridge : MonoBehaviour
{
    public Animator animator;
    public PlatformerController player;

    [Header("Walk Playback")]
    [Tooltip("Ground speed at which the walk clip's feet look planted at 1x playback.")]
    public float walkClipSpeed = 3f;
    public float minPlaybackSpeed = 0.5f;
    public float maxPlaybackSpeed = 2f;

    static readonly int SpeedHash = Animator.StringToHash("Speed");
    static readonly int GroundedHash = Animator.StringToHash("IsGrounded");
    static readonly int WalkMultHash = Animator.StringToHash("WalkSpeedMult");

    void Awake()
    {
        if (player == null) player = GetComponent<PlatformerController>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        float speed = player.HorizontalSpeed;

        animator.SetFloat(SpeedHash, speed);
        animator.SetBool(GroundedHash, player.IsGrounded);

        // Scales Walk playback with movement speed so feet don't skate (sprint plays faster)
        float mult = Mathf.Clamp(speed / walkClipSpeed, minPlaybackSpeed, maxPlaybackSpeed);
        animator.SetFloat(WalkMultHash, mult);
    }
}