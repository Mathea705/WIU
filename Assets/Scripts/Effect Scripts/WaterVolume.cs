using UnityEngine;


public class WaterVolume : MonoBehaviour
{
    private float _surfaceY;
    private SwimController _swimmer;

    void Awake()
    {
        Collider col   = GetComponent<Collider>();
        _surfaceY  = col.bounds.max.y;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _swimmer = other.GetComponentInChildren<SwimController>()
                ?? other.GetComponent<SwimController>();
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") || _swimmer == null) return;
        if (_swimmer.IsSwimming) return;
        if (other.bounds.center.y < _surfaceY)
            _swimmer.EnterWater(_surfaceY);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _swimmer?.ExitWater();
        _swimmer = null;
    }
}
