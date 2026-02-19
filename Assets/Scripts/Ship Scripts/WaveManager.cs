using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float frequency = 0.5f;
    [SerializeField] private float speed     = 1f;

    private void Awake() => instance = this;

    public float GetWaveHeight(float x)
    {
        return amplitude * Mathf.Sin(x * frequency + Time.time * speed);
    }
}
