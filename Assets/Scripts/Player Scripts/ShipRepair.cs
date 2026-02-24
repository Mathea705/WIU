using UnityEngine;
using UnityEngine.UI;

public class ShipRepair : MonoBehaviour
{
    [SerializeField] private HealthSystem shipHealth;
    [SerializeField] private float        healPerSecond = 15f;
    [SerializeField] private GameObject   repairPrompt;

    private bool _inRange;

    private void Update()
    {
        if (!_inRange) return;

        if (Input.GetKey(KeyCode.E))
            shipHealth.Heal(healPerSecond * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _inRange = true;
        if (repairPrompt != null) repairPrompt.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _inRange = false;
        if (repairPrompt != null) repairPrompt.SetActive(false);
    }
}
