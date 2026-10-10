
using UnityEngine;

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
    private Vector3 prevPos;
    private Quaternion prevRot;

    void Start()
    {
        prevPos = transform.position;
        prevRot = transform.rotation;
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

            if (Vector3.Distance(transform.position, target.position) < 0.05f)
            {
                targetIndex = (targetIndex + 1) % waypoints.Length;
                waitTimer = waitTime;
            }
        }

        // Calculate the platform's movement since the previous frame.
        LastDelta = transform.position - prevPos;

        LastRotationDelta =
            transform.rotation * Quaternion.Inverse(prevRot);

        // Always update previous transform values.
        prevPos = transform.position;
        prevRot = transform.rotation;
    }
}