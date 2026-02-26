using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float           maxHealth   = 100f;
    [SerializeField] private float           lerpSpeed   = 5f;
    [SerializeField] private Image           healthBar;
    [SerializeField] private ParticleSystem  smokePrefab;
    [SerializeField] private Transform       smokePoint;

    [SerializeField] private float smokeFadeTime = 1f;
    [SerializeField] private bool  triggerDeathScene = true;


    private ParticleSystem _smokeInstance;
    private Coroutine      _smokeFade;



    public float CurrentHealth => _currentHealth;
    public float Ratio         => _currentHealth / maxHealth;
    public bool  IsAlive       => _currentHealth > 0f;

    private float _currentHealth;
    private float _displayHealth;

    private bool isDead;

    void Awake()
    {
        _currentHealth = maxHealth;
        _displayHealth = maxHealth;

        isDead = false;
    }

    private void OnEnable()
    {
        _currentHealth = maxHealth;
        _displayHealth = maxHealth;
        isDead         = false;
    }

    void Start()
    {

    }

    void Update()
    {
        _displayHealth = Mathf.Lerp(_displayHealth, _currentHealth, Time.deltaTime * lerpSpeed);


        if (healthBar != null)
            healthBar.fillAmount = _displayHealth / maxHealth;

        if (smokePrefab != null && smokePoint != null)
        {
            bool lowHealth = _currentHealth < maxHealth * 0.5f;
            if (lowHealth && _smokeInstance == null)
            {
                _smokeInstance = Instantiate(smokePrefab, smokePoint.position, Quaternion.Euler(-90.0f, 0.0f, 0.0f), smokePoint);
                if (_smokeFade != null) StopCoroutine(_smokeFade);
                _smokeFade = StartCoroutine(FadeSmoke(0f, 1f));
            }
            else if (!lowHealth && _smokeInstance != null)
            {
                if (_smokeFade != null) StopCoroutine(_smokeFade);
                _smokeFade = StartCoroutine(FadeSmoke(1f, 0f, destroyAfter: true));
            }
        }

        if (_currentHealth <= 0 && triggerDeathScene && !isDead)
        {
            isDead = true;
            SceneManager.LoadScene("DeathScene");
        }
    }

    private IEnumerator FadeSmoke(float from, float to, bool destroyAfter = false)
    {
        ParticleSystem target = _smokeInstance;
        if (destroyAfter) _smokeInstance = null;

        float t = 0f;
        while (t < smokeFadeTime)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, t / smokeFadeTime);
            if (target != null)
            {
                var main = target.main;
                Color c  = main.startColor.color;
                c.a      = alpha;
                main.startColor = c;
            }
            yield return null;
        }
        if (destroyAfter && target != null)
            Destroy(target.gameObject);
    }

    public void TakeDamage(float amount)
    {
        _currentHealth = Mathf.Max(0f, _currentHealth - amount);
    }

    public void Heal(float amount)
    {
        _currentHealth = Mathf.Min(maxHealth, _currentHealth + amount);
    }

    public void HealFull()
    {
        _currentHealth = maxHealth;
    }
}
