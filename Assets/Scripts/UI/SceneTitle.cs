using System.Collections;
using TMPro;
using UnityEngine;

public class SceneTitle : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [TextArea]
    [SerializeField] private string   message    = "Scene Name";
    [SerializeField] private float    fadeInTime  = 1f;
    [SerializeField] private float    holdTime    = 2f;
    [SerializeField] private float    fadeOutTime = 1f;

    private void Start()
    {
        label.text = message;
        SetAlpha(0f);
        StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        yield return Fade(0f, 1f, fadeInTime);
        yield return new WaitForSeconds(holdTime);
        yield return Fade(1f, 0f, fadeOutTime);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, t / duration));
            yield return null;
        }
        SetAlpha(to);
    }

    private void SetAlpha(float alpha)
    {
        Color c = label.color;
        c.a = alpha;
        label.color = c;
    }
}
