using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossAI : MonoBehaviour
{
    [SerializeField] protected float maxHealth = 200f;

    protected GameObject shipObject;

    [SerializeField] private GameObject bossHealthBarPanel;
    [SerializeField] private Image bossHealthBarFill;
    [SerializeField] private float barLerpSpeed = 5f;

    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private float deathShakeDuration = 5f;
    [SerializeField] private float deathShakeIntensity = 0.4f;

    protected float currentHealth;
    protected HealthSystem shipHealth;
    protected bool _invulnerable = false;

    private float _displayHealth;
    private float _lastImpactSoundTime = -999f;
    private Renderer[] _renderers;
    private Color[] _originalColors;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        _displayHealth = maxHealth;

        shipObject = GameObject.FindWithTag("Ship");


         shipHealth = shipObject.GetComponent<HealthSystem>();

        _renderers = GetComponentsInChildren<Renderer>();
        _originalColors = new Color[_renderers.Length];
        for (int i = 0; i < _renderers.Length; i++)
            _originalColors[i] = _renderers[i].material.color;

        if (bossHealthBarPanel != null)
            bossHealthBarPanel.SetActive(false);
    }

    protected virtual void Update()
    {
        if (bossHealthBarFill == null) return;

        _displayHealth = Mathf.Lerp(_displayHealth, currentHealth, Time.deltaTime * barLerpSpeed);
        bossHealthBarFill.fillAmount = _displayHealth / maxHealth;

        Debug.Log(bossHealthBarFill.fillAmount);
    }

    public void TakeDamage(float amount)
    {
        if (_invulnerable) return;



        if (bossHealthBarPanel != null)
            bossHealthBarPanel.SetActive(true);

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        StartCoroutine(FlashRed());

        if (currentHealth <= 0f)
            OnDeath();
    }

    private IEnumerator FlashRed()
    {
        float half = flashDuration * 0.5f;
        float t = 0f;

        while (t < half)
        {
            t += Time.deltaTime;
            float p = t / half;
            for (int i = 0; i < _renderers.Length; i++)
                _renderers[i].material.color = Color.Lerp(_originalColors[i], Color.red, p);
            yield return null;
        }

        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float p = t / half;
            for (int i = 0; i < _renderers.Length; i++)
                _renderers[i].material.color = Color.Lerp(Color.red, _originalColors[i], p);
            yield return null;
        }

        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].material.color = _originalColors[i];
    }

    public void DealDamageToShip(float amount)
    {
        if (Time.time >= _lastImpactSoundTime + 1.0f)
        {
            AudioManager.Instance.Play("BoatImpactSFX");
            _lastImpactSoundTime = Time.time;
        }
        shipHealth.TakeDamage(amount);
    }

    protected virtual void OnDeath()
    {
        if (bossHealthBarPanel != null)
            bossHealthBarPanel.SetActive(false);

        _invulnerable = true;
        StopAllCoroutines();
        StartCoroutine(DeathShake());
    }

    private IEnumerator DeathShake()
    {
        Vector3 origin = transform.position;
        float t = 0f;

        while (t < deathShakeDuration)
        {
            t += Time.deltaTime;
            transform.position = origin + Random.insideUnitSphere * deathShakeIntensity;
            yield return null;
        }

        Destroy(gameObject);
    }
}
