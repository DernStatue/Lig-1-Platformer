// Small script to feed PlatformerController state into the Animator
using UnityEngine;

public class AnimatorBridge : MonoBehaviour
{
    public Animator animator;
    public PlatformerController player;

    void Update()
    {
        animator.SetFloat("Speed", player.HorizontalVelocity.magnitude);
        animator.SetBool("IsGrounded", player.IsGrounded);
    }
}