using UnityEngine;

/// <summary>
/// Controls the underwater fullscreen shader graph effect.
///
/// Setup:
///   1. Add a "Full Screen Pass Renderer Feature" to your URP Renderer asset.
///   2. Assign your underwater shader material to it.
///   3. Assign that same material to the "Underwater Material" field below.
///   4. Assign SwimController.
/// </summary>
public class UnderwaterFX : MonoBehaviour
{
    [SerializeField] private SwimController swimController;
    [SerializeField] private Camera         playerCamera;

    [Header("Shader Material")]
    [SerializeField] private Material underwaterMaterial;
    [SerializeField] private float    fxTransitionSpeed = 4f;

    [Header("Camera")]
    [SerializeField] private Color underwaterBackgroundColor = new Color(0.01f, 0.12f, 0.18f);

    [Header("Fog")]
    [SerializeField] private Color underwaterFogColor   = new Color(0.01f, 0.15f, 0.22f);
    [SerializeField] private float underwaterFogDensity = 0.09f;
    [SerializeField] private float fogTransitionSpeed   = 3f;

    [Header("Depth Vignette")]
    [SerializeField] private float maxDarkDepth         = 10f;
    [SerializeField] private float minVignetteIntensity = 0.2f;
    [SerializeField] private float maxVignetteIntensity = 0.65f;

    private static readonly int _intensityID        = Shader.PropertyToID("_Intensity");
    private static readonly int _vignetteIntensityID = Shader.PropertyToID("_VignetteIntensity");

    private bool         _wasUnderwater;
    private float        _savedFogDensity;
    private Color        _savedFogColor;
    private bool         _savedFogEnabled;
    private FogMode      _savedFogMode;
    private CameraClearFlags _savedClearFlags;
    private Color        _savedBackgroundColor;

    void Start()
    {
        _savedFogEnabled = RenderSettings.fog;
        _savedFogColor   = RenderSettings.fogColor;
        _savedFogDensity = RenderSettings.fogDensity;
        _savedFogMode    = RenderSettings.fogMode;

        if (underwaterMaterial != null)
            underwaterMaterial.SetFloat(_intensityID, 0f);

        if (playerCamera != null)
        {
            _savedClearFlags       = playerCamera.clearFlags;
            _savedBackgroundColor  = playerCamera.backgroundColor;
        }
    }

    void Update()
    {
        bool underwater = swimController.IsUnderwater;

        if (underwater != _wasUnderwater)
        {
            if (underwater) OnEnterUnderwater();
            else            OnExitUnderwater();
            _wasUnderwater = underwater;
        }

        // Fade shader in/out
        float target  = underwater ? 1f : 0f;
        float current = underwaterMaterial.GetFloat(_intensityID);
        underwaterMaterial.SetFloat(_intensityID,
            Mathf.Lerp(current, target, Time.deltaTime * fxTransitionSpeed));

        if (!underwater) return;

        float depth = swimController.WaterDepth;

        // Lerp fog toward underwater values
        RenderSettings.fogDensity = Mathf.Lerp(
            RenderSettings.fogDensity, underwaterFogDensity, Time.deltaTime * fogTransitionSpeed);
        RenderSettings.fogColor = Color.Lerp(
            RenderSettings.fogColor, underwaterFogColor, Time.deltaTime * fogTransitionSpeed);

        // Drive vignette intensity with depth
        float t        = Mathf.Clamp01(depth / maxDarkDepth);
        float vignette = Mathf.Lerp(minVignetteIntensity, maxVignetteIntensity, t);
        underwaterMaterial.SetFloat(_vignetteIntensityID, vignette);
    }

    private void OnEnterUnderwater()
    {
        RenderSettings.fog     = true;
        RenderSettings.fogMode = FogMode.Exponential;

        if (playerCamera != null)
        {
            playerCamera.clearFlags       = CameraClearFlags.SolidColor;
            playerCamera.backgroundColor  = underwaterBackgroundColor;
        }
    }

    private void OnExitUnderwater()
    {
        RenderSettings.fog        = _savedFogEnabled;
        RenderSettings.fogColor   = _savedFogColor;
        RenderSettings.fogDensity = _savedFogDensity;
        RenderSettings.fogMode    = _savedFogMode;

        if (playerCamera != null)
        {
            playerCamera.clearFlags      = _savedClearFlags;
            playerCamera.backgroundColor = _savedBackgroundColor;
        }
    }
}
