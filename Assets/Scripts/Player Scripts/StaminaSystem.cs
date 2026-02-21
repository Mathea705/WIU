using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{
    [SerializeField] private float maxStamina  = 100f;
    [SerializeField] private float drainRate   = 20f;
    [SerializeField] private float swimDrainRate = 12f;
    [SerializeField] private float regenRate  = 10f;
    [SerializeField] private float jumpCost    = 25f;
    [SerializeField] private float drowningDrainRate = 8f;
    [SerializeField] private Image  staminaBar;
    [SerializeField] private RectTransform flashIcon;
    [SerializeField] private HealthSystem  health;   

    public bool  CanRun    => !_isExhausted;
    public bool  IsRunning => _isRunning;
    public float Ratio     => _stamina / maxStamina;

    private float _stamina;
    private bool  _isExhausted;
    private bool  _isRunning;
    private bool  _isSwimming;
    private bool  _wasExhausted;

    void Awake()
    {
        _stamina = maxStamina;
    }

    void Update()
    {
        _isRunning = Input.GetKey(KeyCode.LeftShift) && CanRun;

        if (_isRunning || _isSwimming)
        {
            float rate = _isSwimming ? swimDrainRate : drainRate;
            _stamina -= rate * Time.deltaTime;
            if (_stamina <= 0f)
            {
                _stamina     = 0f;
                _isExhausted = true;
            }
        }
        else
        {
            _stamina = Mathf.Min(_stamina + regenRate * Time.deltaTime, maxStamina);
            if (_isExhausted && _stamina >= maxStamina * 0.5f)
                _isExhausted = false;
        }

        if (_isSwimming && _isExhausted && health != null)
            health.TakeDamage(drowningDrainRate * Time.deltaTime);

        if (_isExhausted && !_wasExhausted)
            StartCoroutine(FlashIcon());
        _wasExhausted = _isExhausted;

        staminaBar.fillAmount = _stamina / maxStamina;
    }

    public void SetSwimming(bool swimming) => _isSwimming = swimming;

    public void ApplyDiverBoost() => swimDrainRate *= 0.2f;

    public void OnJump()
    {
        _stamina = Mathf.Max(0f, _stamina - jumpCost);
        if (_stamina == 0f)
            _isExhausted = true;
    }

    private IEnumerator FlashIcon()
    {

        GameObject copy  = Instantiate(flashIcon.gameObject, flashIcon.parent);
        RectTransform rt = copy.GetComponent<RectTransform>();
        Image img  = copy.GetComponent<Image>();

        rt.anchoredPosition = flashIcon.anchoredPosition;
        rt.sizeDelta = flashIcon.sizeDelta;

        Vector3 startScale = flashIcon.localScale;
        Vector3 endScale = startScale * 1.5f;

        float duration = 0.4f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            rt.localScale = Vector3.Lerp(startScale, endScale, t);

           
                Color c = img.color;
                c.a = Mathf.Lerp(1f, 0f, t);
                img.color = c;
        

            yield return null;
        }

        Destroy(copy);
    }
}
