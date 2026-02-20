using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Attach to any object that should disturb the water surface (ship, debris, fish, etc.).
/// Creates an invisible flat marker quad on the ripple layer so the top-down
/// ripple camera picks it up and injects a disturbance at this object's position.
/// </summary>
public class WaterRippleSource : MonoBehaviour
{
    [Tooltip("Must match the layer the ripple camera's culling mask renders. " +
             "Check the ripple camera's Culling Mask in the inspector: " +
             "layer index = log2(cullingMaskValue). e.g. culling mask 2 → Layer 1.")]
    public int rippleLayer = 1;

    [Tooltip("Size of the water-contact footprint in world units (X = width, Y = length).")]
    public Vector2 footprintSize = new Vector2(2f, 5f);

    [Tooltip("Local XZ offset from this object's pivot to the water-contact centre. " +
             "Use this to correct any remaining position drift.")]
    public Vector2 pivotOffset;

    [Tooltip("Local Y offset so the marker sits at water surface level " +
             "(negative if the pivot is above the waterline).")]
    public float heightOffset = 0f;

    private GameObject _marker;
    private MeshRenderer _markerRenderer;

    void Awake()
    {
        _marker = GameObject.CreatePrimitive(PrimitiveType.Quad);
        _marker.name = "RippleMarker_" + gameObject.name;
        _marker.layer = rippleLayer;

        // Quads default to facing +Z; rotate so it faces +Y (seen by top-down camera).
        _marker.transform.SetParent(transform, false);
        _marker.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        ApplyTransform();

        // Unlit white so it registers in the RFloat render texture.
        _markerRenderer = _marker.GetComponent<MeshRenderer>();
        _markerRenderer.material = new Material(Shader.Find("Unlit/Color"));
        _markerRenderer.material.color = Color.white;
        _markerRenderer.shadowCastingMode = ShadowCastingMode.Off;
        _markerRenderer.receiveShadows = false;

        // Remove the collider – it's a visual-only marker.
        Destroy(_marker.GetComponent<MeshCollider>());

        // Exclude this layer from the main camera so the marker is invisible
        // to everything except the ripple camera.
        if (Camera.main != null)
            Camera.main.cullingMask &= ~(1 << rippleLayer);
    }

    void OnDestroy()
    {
        if (_marker != null)
            Destroy(_marker);
    }

    // Lets you tweak size/offset in Play mode and see it update immediately.
    void OnValidate()
    {
        if (_marker != null)
            ApplyTransform();
    }

    void ApplyTransform()
    {
        _marker.transform.localPosition = new Vector3(pivotOffset.x, heightOffset, pivotOffset.y);
        _marker.transform.localScale    = new Vector3(footprintSize.x, footprintSize.y, 1f);
    }
}
