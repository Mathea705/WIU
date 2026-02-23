using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// Attach this directly to the LoadingCanvas GameObject.
public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [SerializeField] private Image            background;
    [SerializeField] private Image            barFill;
    [SerializeField] private TextMeshProUGUI  loadingText;
    [SerializeField] private float            fadeDuration = 0.5f;
    [SerializeField] private float            minShowTime  = 1.5f;
    [SerializeField] private float            dotInterval  = 0.4f;

    private Canvas _canvas;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;   // hide visually — GameObject stays active so coroutines work
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(Transition(sceneName));
    }

    private IEnumerator Transition(string sceneName)
    {
        _canvas.enabled = true;
        if (barFill != null) barFill.fillAmount = 0f;
        SetBgAlpha(0f);

        Coroutine dots = null;
        if (loadingText != null) dots = StartCoroutine(AnimateDots());

        yield return StartCoroutine(Fade(1f));

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float startTime = Time.unscaledTime;

        while (op.progress < 0.9f || Time.unscaledTime - startTime < minShowTime)
        {
            float t = Mathf.Min(op.progress / 0.9f, (Time.unscaledTime - startTime) / minShowTime);
            if (barFill != null) barFill.fillAmount = t;
            yield return null;
        }

        if (barFill != null) barFill.fillAmount = 1f;
        op.allowSceneActivation = true;
        yield return null;

        if (dots != null) StopCoroutine(dots);
        if (loadingText != null) loadingText.text = "Loading...";

        yield return StartCoroutine(Fade(0f));
        _canvas.enabled = false;
    }

    private IEnumerator AnimateDots()
    {
        int count = 0;
        while (true)
        {
            count = (count % 3) + 1;
            loadingText.text = "Loading" + new string('.', count);
            yield return new WaitForSeconds(dotInterval);
        }
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float start   = background.color.a;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            SetBgAlpha(Mathf.Lerp(start, targetAlpha, elapsed / fadeDuration));
            yield return null;
        }
        SetBgAlpha(targetAlpha);
    }

    private void SetBgAlpha(float a)
    {
        Color c = background.color;
        c.a = a;
        background.color = c;
    }
}
