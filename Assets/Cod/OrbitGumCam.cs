using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Vector3 targetOffset = new Vector3(0, 1.5f, 0);

    [Header("Orbit")]
    public float distance = 6f;
    public float minDistance = 1.5f;
    public float maxDistance = 10f;
    public float rotationSpeed = 3f;
    public float minPitch = -20f;
    public float maxPitch = 60f;

    [Header("Smoothing")]
    public float positionSmoothTime = 0.08f;
    public float rotationSmoothSpeed = 12f;

    [Header("Collision")]
    public LayerMask collisionMask;
    public float collisionRadius = 0.2f;
    public float collisionBuffer = 0.15f;

    private float yaw;
    private float pitch = 15f;
    private Vector3 velocity;
    private float currentDistance;

    void Start()
    {
        currentDistance = distance;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Mouse input (swap for new Input System if that's what you're using)
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
        Vector3 pivot = target.position + targetOffset;

        // Desired camera position before collision check
        Vector3 desiredPos = pivot - rot * Vector3.forward * distance;

        // Collision: shrink distance if something's in the way
        float targetDist = distance;
        if (Physics.SphereCast(pivot, collisionRadius, (desiredPos - pivot).normalized,
                out RaycastHit hit, distance, collisionMask))
        {
            targetDist = Mathf.Clamp(hit.distance - collisionBuffer, minDistance, distance);
        }
        currentDistance = Mathf.Lerp(currentDistance, targetDist, 15f * Time.deltaTime);

        Vector3 finalPos = pivot - rot * Vector3.forward * currentDistance;

        transform.position = Vector3.SmoothDamp(transform.position, finalPos, ref velocity, positionSmoothTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSmoothSpeed * Time.deltaTime);
    }
}