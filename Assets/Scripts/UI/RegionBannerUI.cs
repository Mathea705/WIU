using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RegionBannerUI : MonoBehaviour
{
    public static RegionBannerUI Instance { get; private set; }

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text    regionNameText;
    [SerializeField] private Image       leftDecor;
    [SerializeField] private Image       rightDecor;

    [SerializeField] private float fadeInDuration  = 0.6f;
    [SerializeField] private float holdDuration    = 2.5f;
    [SerializeField] private float fadeOutDuration = 0.8f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        canvasGroup.alpha = 0f;
    }

    public void Show(string regionName)
    {
        if (string.IsNullOrEmpty(regionName)) return;
        StopAllCoroutines();
        regionNameText.text = regionName;
        StartCoroutine(PlayBanner());
    }

    private IEnumerator PlayBanner()
    {
        yield return Fade(0f, 1f, fadeInDuration);
        yield return new WaitForSeconds(holdDuration);
        yield return Fade(1f, 0f, fadeOutDuration);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}
