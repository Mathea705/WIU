using System.Collections;
using UnityEngine;

public class WhirlpoolHazard : MonoBehaviour
{
    [SerializeField] private float damagePerSecond = 10f;
    [SerializeField] private float rotateSpeed     = 120f;  // degrees per second, clockwise from above
    [SerializeField] private float fadeTime        = 1f;

    private HealthSystem  _shipHealth;
    private bool          _shipInside;
    private Renderer[]    _renderers;

    private void Start()
    {
        GameObject ship = GameObject.FindWithTag("Ship");
        if (ship != null)
            _shipHealth = ship.GetComponent<HealthSystem>();

        _renderers = GetComponentsInChildren<Renderer>();
        SetAlpha(0f);
        StartCoroutine(Fade(0f, 1f));
    }

    private void Update()
    {
        transform.Rotate(Vector3.down, rotateSpeed * Time.deltaTime);

        if (_shipInside && _shipHealth != null)
            _shipHealth.TakeDamage(damagePerSecond * Time.deltaTime);
    }

    // Called by the Leviathan instead of Destroy so the fade plays first
    public void Dissolve()
    {
        _shipInside = false; // stop dealing damage during fade
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        yield return Fade(1f, 0f);
        Destroy(gameObject);
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, t / fadeTime));
            yield return null;
        }
        SetAlpha(to);
    }

    private void SetAlpha(float alpha)
    {
        foreach (Renderer r in _renderers)
        {
            Color c = r.material.color;
            c.a = alpha;
            r.material.color = c;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ship")) _shipInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ship")) _shipInside = false;
    }
}
