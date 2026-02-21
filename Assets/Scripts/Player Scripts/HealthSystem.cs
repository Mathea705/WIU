using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth  = 100f;
    [SerializeField] private float lerpSpeed  = 5f;
    [SerializeField] private Image healthBar;

      [SerializeField] private RectTransform flashIcon;

    public float CurrentHealth => _currentHealth;
    public float Ratio         => _currentHealth / maxHealth;
    public bool  IsAlive       => _currentHealth > 0f;

    private float _currentHealth;
    private float _displayHealth;

    void Awake()
    {
        _currentHealth = maxHealth;
        _displayHealth = maxHealth;
    }

    void Update()
    {
        _displayHealth = Mathf.Lerp(_displayHealth, _currentHealth, Time.deltaTime * lerpSpeed);


            healthBar.fillAmount = _displayHealth / maxHealth;

            if (_currentHealth <= 0)
            {
                Debug.Log("Player died!");

            }
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
