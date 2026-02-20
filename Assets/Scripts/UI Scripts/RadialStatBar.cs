using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadialStatBar : MonoBehaviour
{
    [SerializeField] private Image    fillImage;

    private float _max = 100f;
    private float _current = 100f;

    public float Current => _current;
    public float Max     => _max;
    public float Ratio   => _max > 0f ? _current / _max : 0f;

    public void SetValue(float current, float max)
    {
        _max  = max;
        _current = Mathf.Clamp(current, 0f, _max);
        Refresh();
    }

    public void SetValue(float current)
    {
        _current = Mathf.Clamp(current, 0f, _max);
        Refresh();
    }

    public void SetMax(float max)
    {
        _max = max;
        Refresh();
    }

    private void Refresh()
    {
       
            fillImage.fillAmount = Ratio;

    
    }
}
