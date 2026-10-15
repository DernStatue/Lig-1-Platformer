
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PlatformTextureTiling : MonoBehaviour
{
    [Header("Texture Tiling")]
    [Tooltip("Texture repeats per world unit along the X axis.")]
    [Min(0.01f)]
    public float horizontalTilesPerUnit = 1f;

    [Tooltip("Texture repeats per world unit along the Z axis.")]
    [Min(0.01f)]
    public float verticalTilesPerUnit = 1f;

    [Tooltip("Texture property used by URP Lit.")]
    public string textureProperty = "_BaseMap";

    private Renderer platformRenderer;
    private Material platformMaterial;

    void Awake()
    {
        platformRenderer = GetComponent<Renderer>();

        // Give this platform its own material instance.
        platformMaterial = platformRenderer.material;

        UpdateTiling();
    }

    public void UpdateTiling()
    {
        if (platformRenderer == null)
            platformRenderer = GetComponent<Renderer>();

        if (platformMaterial == null)
            platformMaterial = platformRenderer.material;

        Vector3 size = transform.lossyScale;

        Vector2 tiling = new Vector2(
            Mathf.Max(0.01f, size.x * horizontalTilesPerUnit),
            Mathf.Max(0.01f, size.z * verticalTilesPerUnit)
        );

        platformMaterial.SetTextureScale(textureProperty, tiling);
    }

    void OnDestroy()
    {
        if (platformMaterial != null)
        {
            if (Application.isPlaying)
                Destroy(platformMaterial);
            else
                DestroyImmediate(platformMaterial);
        }
    }
}
