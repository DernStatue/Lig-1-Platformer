using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class WalkCycleRig : MonoBehaviour
{
    [System.Serializable]
    public class LimbPivots
    {
        public Transform upperPivot; // hip or shoulder
        public Transform lowerPivot; // knee or elbow
    }

    [Header("Legs")]
    public LimbPivots leftLeg;
    public LimbPivots rightLeg;

    [Header("Arms")]
    public LimbPivots leftArm;
    public LimbPivots rightArm;

    [Header("Walk Motion")]
    public float swingAngle = 30f;
    public float kneeBendAngle = 50f;
    public float elbowBendAngle = 25f;
    public float cycleSpeed = 6f;
    public float maxWalkSpeed = 4f;

    [Header("Jump Pose")]
    public float jumpKneeTuck = 70f;
    public float jumpElbowRaise = 30f;
    public float airPoseBlendSpeed = 6f;

    public PlatformerController player;

    private float cyclePhase;
    private float airBlend;

    void Update()
    {
        float speed = player.HorizontalVelocity.magnitude;
        bool grounded = player.IsGrounded;
        airBlend = Mathf.Lerp(airBlend, grounded ? 0f : 1f, airPoseBlendSpeed * Time.deltaTime);

        if (speed > 0.1f && grounded)
            cyclePhase += speed * cycleSpeed * Time.deltaTime;

        float speedScale = Mathf.Clamp01(speed / maxWalkSpeed);
        float vertVel = player.VerticalVelocity;

        AnimateLeg(leftLeg, cyclePhase, speedScale, vertVel);
        AnimateLeg(rightLeg, cyclePhase + Mathf.PI, speedScale, vertVel);
        AnimateArm(leftArm, cyclePhase + Mathf.PI, speedScale, vertVel);
        AnimateArm(rightArm, cyclePhase, speedScale, vertVel);
    }

    void AnimateLeg(LimbPivots leg, float phase, float speedScale, float vertVel)
    {
        float swing = Mathf.Sin(phase) * swingAngle * speedScale;
        float lift = Mathf.Max(0, Mathf.Sin(phase));
        float knee = lift * kneeBendAngle;

        float airHip = vertVel > 0.5f ? -40f : (vertVel < -0.5f ? 15f : 0f);
        float airKnee = vertVel > 0.5f ? jumpKneeTuck : (vertVel < -0.5f ? 10f : 0f);

        float hipAngle = Mathf.Lerp(swing, airHip, airBlend);
        float kneeAngle = Mathf.Lerp(knee, airKnee, airBlend);

        leg.upperPivot.localRotation = Quaternion.Euler(hipAngle, 0, 0);
        leg.lowerPivot.localRotation = Quaternion.Euler(-kneeAngle, 0, 0);
    }

    void AnimateArm(LimbPivots arm, float phase, float speedScale, float vertVel)
    {
        float swing = Mathf.Sin(phase) * (swingAngle * 0.7f) * speedScale;
        float elbow = Mathf.Abs(Mathf.Sin(phase)) * elbowBendAngle;

        float shoulderAngle = Mathf.Lerp(swing, jumpElbowRaise, airBlend);
        float elbowAngle = Mathf.Lerp(elbow, jumpElbowRaise * 0.5f, airBlend);

        arm.upperPivot.localRotation = Quaternion.Euler(shoulderAngle, 0, 0);
        arm.lowerPivot.localRotation = Quaternion.Euler(-elbowAngle, 0, 0);
    }
}