
using UnityEngine;

[RequireComponent(typeof(PlatformerController))]
public class PlatformerAnimationBridge : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    [Header("Animation")]
    public float animationDampTime = 0.1f;

    private PlatformerController movement;

    void Awake()
    {
        movement = GetComponent<PlatformerController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (animator == null || movement == null)
            return;

        // Use horizontal movement speed, ignoring jumping/falling.
        float speed = movement.HorizontalSpeed;

        animator.SetFloat(
            "Speed",
            speed,
            animationDampTime,
            Time.deltaTime
        );
    }
}