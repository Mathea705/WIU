using UnityEngine;

public class PlayerFXController : MonoBehaviour
{
    [SerializeField] private HealthSystem  health;
    [SerializeField] private StaminaSystem stamina;
    [SerializeField] private Material      fxMaterial;

    [SerializeField] private float blurThreshold = 0.35f;
    [SerializeField] private float maxBlur  = 1f;

    [SerializeField] private float vignetteThreshold = 0.4f;
    [SerializeField] private float maxVignette  = 0.75f;

    [SerializeField] private float maxChromatic = 0.03f;

    [SerializeField] private float lerpSpeed = 4f;

    private static readonly int BlurProp = Shader.PropertyToID("_BlurAmount");
    private static readonly int VignetteProp  = Shader.PropertyToID("_VignetteAmount");
    private static readonly int ChromaticProp = Shader.PropertyToID("_ChromaticAmount");

    void Update()
    {
        Material mat = fxMaterial;
        if (mat == null) return;

        float healthRatio = health  != null ? health.Ratio  : 1f;
        float staminaRatio = stamina != null ? stamina.Ratio : 1f;

        float targetBlur = Mathf.Clamp01(1f - staminaRatio / blurThreshold)     * maxBlur;
        float targetVignette = Mathf.Clamp01(1f - healthRatio  / vignetteThreshold) * maxVignette;
        float targetChromatic = Mathf.Clamp01(1f - healthRatio  / vignetteThreshold) * maxChromatic;

        mat.SetFloat(BlurProp,Mathf.Lerp(mat.GetFloat(BlurProp), targetBlur,  Time.deltaTime * lerpSpeed));
        mat.SetFloat(VignetteProp,  Mathf.Lerp(mat.GetFloat(VignetteProp),  targetVignette,  Time.deltaTime * lerpSpeed));
        mat.SetFloat(ChromaticProp, Mathf.Lerp(mat.GetFloat(ChromaticProp), targetChromatic, Time.deltaTime * lerpSpeed));
    }
}
