using System.Collections;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [SerializeField] protected float maxHealth = 200f;

    [SerializeField] protected GameObject shipObject;

    [SerializeField] private float flashDuration = 0.2f;

    protected float currentHealth;
    protected HealthSystem shipHealth;

    private Renderer[] _renderers;
    private Color[] _originalColors;

    protected virtual void Start()
    {
        currentHealth = maxHealth;

        if (shipObject != null)
            shipHealth = shipObject.GetComponent<HealthSystem>();

        _renderers = GetComponentsInChildren<Renderer>();
        _originalColors = new Color[_renderers.Length];
        for (int i = 0; i < _renderers.Length; i++)
            _originalColors[i] = _renderers[i].material.color;
    }

    public void TakeDamage(float amount)
    {
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

    protected void DealDamageToShip(float amount)
    {
     
            shipHealth.TakeDamage(amount);
    }

    protected virtual void OnDeath()
    {
        Destroy(gameObject);
    }
}
