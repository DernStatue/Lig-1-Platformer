using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 45f, 0);

    public Quaternion LastRotationDelta { get; private set; } = Quaternion.identity;
    private Quaternion prevRot;

    void Start() => prevRot = transform.rotation;

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
        LastRotationDelta = transform.rotation * Quaternion.Inverse(prevRot);
        prevRot = transform.rotation;
    }
}