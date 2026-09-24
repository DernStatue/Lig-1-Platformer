using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 3f;
    public float waitTime = 0.5f;

    public Vector3 LastDelta { get; private set; }
    public Quaternion LastRotationDelta { get; private set; } = Quaternion.identity;

    private int targetIndex = 0;
    private float waitTimer = 0f;
    private Vector3 prevPos;
    private Quaternion prevRot;

    void Start()
    {
        prevPos = transform.position;
        prevRot = transform.rotation;
    }

    void Update()
    {
        if (waypoints.Length < 2) return;

        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            LastDelta = Vector3.zero;
            LastRotationDelta = Quaternion.identity;
            return;
        }

        Transform target = waypoints[targetIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            targetIndex = (targetIndex + 1) % waypoints.Length;
            waitTimer = waitTime;
        }

        LastDelta = transform.position - prevPos;
        LastRotationDelta = transform.rotation * Quaternion.Inverse(prevRot);
        prevPos = transform.position;
        prevRot = transform.rotation;
    }
}