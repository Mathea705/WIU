using System.Collections;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject mapCanvas;
    [SerializeField] private GameObject[] mapPanels;
    [SerializeField] private float openDuration = 0.25f;

    private bool isActive;
    private const float TargetScaleY = 1.68f;

    void Start()
    {
        isActive = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && isActive)
            DeactivateMap();
        else if (Input.GetKeyDown(KeyCode.M) && !isActive)
            ActivateMap();
    }

    void CheckActive()
    {
        if (isActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }
    }

    void ActivateMap()
    {
        isActive = true;
        StopAllCoroutines();
        mapCanvas.SetActive(true);
        StartCoroutine(AnimateOpen());
        CheckActive();
    }

    void DeactivateMap()
    {
        isActive = false;
        StopAllCoroutines();
        mapCanvas.SetActive(false);
        Vector3 s = mapCanvas.transform.localScale;
        mapCanvas.transform.localScale = new Vector3(s.x, 0f, s.z);
        CheckActive();
    }

    private IEnumerator AnimateOpen()
    {
        Vector3 scale = mapCanvas.transform.localScale;
        scale.y = 0f;
        mapCanvas.transform.localScale = scale;

        float elapsed = 0f;
        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / openDuration);
            scale.y = Mathf.Lerp(0f, TargetScaleY, t);
            mapCanvas.transform.localScale = scale;
            yield return null;
        }

        scale.y = TargetScaleY;
        mapCanvas.transform.localScale = scale;
    }

    private void ShowPanel(int index)
    {
        for (int i = 0; i < mapPanels.Length; i++)
            mapPanels[i].SetActive(false);

        GameObject panel = mapPanels[index];
        panel.SetActive(true);
        StartCoroutine(AnimatePanel(panel));
    }

    private IEnumerator AnimatePanel(GameObject panel)
    {
        Vector3 scale = panel.transform.localScale;
        scale.y = 0f;
        panel.transform.localScale = scale;

        float elapsed = 0f;
        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / openDuration);
            scale.y = Mathf.Lerp(0f, TargetScaleY, t);
            panel.transform.localScale = scale;
            yield return null;
        }

        scale.y = TargetScaleY;
        panel.transform.localScale = scale;
    }

    public void DisplayDocks()     => ShowPanel(0);
    public void DisplayBali()      => ShowPanel(1);
    public void DisplayTasmania()  => ShowPanel(2);
    public void DisplayBermuda()   => ShowPanel(3);
    public void DisplaySunkenCity() => ShowPanel(4);
}
