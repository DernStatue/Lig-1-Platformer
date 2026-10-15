
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class RotatingPlatform : MonoBehaviour
{
    [Header("Rotation")]
    public Vector3 localRotationAxis = Vector3.up;
    public float rotationSpeed = 45f;

    public Vector3 LastDelta { get; private set; }
    public Quaternion LastRotationDelta { get; private set; }
        = Quaternion.identity;

    private Vector3 previousPosition;
    private Quaternion previousRotation;

    void Start()
    {
        previousPosition = transform.position;
        previousRotation = transform.rotation;
    }

    void Update()
    {
        if (localRotationAxis.sqrMagnitude > 0.0001f)
        {
            transform.Rotate(
                localRotationAxis.normalized,
                rotationSpeed * Time.deltaTime,
                Space.Self
            );
        }

        LastDelta = transform.position - previousPosition;

        LastRotationDelta =
            transform.rotation * Quaternion.Inverse(previousRotation);

        previousPosition = transform.position;
        previousRotation = transform.rotation;
    }
}
