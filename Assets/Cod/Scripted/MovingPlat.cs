
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class MovingPlatform : MonoBehaviour
{
    [Header("Movement")]
    public Transform[] waypoints;
    public float speed = 3f;
    public float waitTime = 0.5f;

    public Vector3 LastDelta { get; private set; }
    public Quaternion LastRotationDelta { get; private set; }
        = Quaternion.identity;

    private int targetIndex;
    private float waitTimer;
    private Vector3 previousPosition;
    private Quaternion previousRotation;

    void Start()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        targetIndex = waypoints.Length > 1 ? 1 : 0;

        previousPosition = transform.position;
        previousRotation = transform.rotation;
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length < 2)
        {
            LastDelta = Vector3.zero;
            LastRotationDelta = Quaternion.identity;
            return;
        }

        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
        }
        else
        {
            Transform target = waypoints[targetIndex];

            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, target.position)
                < 0.05f)
            {
                targetIndex = (targetIndex + 1) % waypoints.Length;
                waitTimer = waitTime;
            }
        }

        RecordMovement();
    }

    void RecordMovement()
    {
        LastDelta = transform.position - previousPosition;

        LastRotationDelta =
            transform.rotation * Quaternion.Inverse(previousRotation);

        previousPosition = transform.position;
        previousRotation = transform.rotation;
    }
}
