using UnityEngine;

public class ShipBob : MonoBehaviour
{
    [Header("Bob")]
    public float bobSmoothSpeed  = 2f;
    public float waterlineOffset = 1f;

    [Header("Tilt")]
    public float tiltAmount      = 8f;
    public float tiltSmoothSpeed = 2f;
    public float sampleDistance  = 2f;
    public float bankAmount      = 0.5f;

    private float _lastYRotation;

    private void Start()
    {
        _lastYRotation = transform.eulerAngles.y;
    }

    private void FixedUpdate()
    {
        float heightCenter = WaveManager.instance.GetWaveHeight(transform.position.x);
        float heightFront  = WaveManager.instance.GetWaveHeight(transform.position.x + sampleDistance);
        float heightBack   = WaveManager.instance.GetWaveHeight(transform.position.x - sampleDistance);

        Vector3 targetPos = new Vector3(transform.position.x, heightCenter + waterlineOffset, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.fixedDeltaTime * bobSmoothSpeed);

        float turnRate   = Mathf.DeltaAngle(_lastYRotation, transform.eulerAngles.y) / Time.fixedDeltaTime;
        float pitchAngle = Mathf.Atan2(heightFront - heightBack, sampleDistance * 2f) * Mathf.Rad2Deg * tiltAmount;
        float rollAngle  = Mathf.Sin(Time.time * 0.7f + transform.position.z * 0.5f) * tiltAmount * 0.5f
                         - turnRate * bankAmount;

        Quaternion targetRot = Quaternion.Euler(pitchAngle, transform.eulerAngles.y, rollAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.fixedDeltaTime * tiltSmoothSpeed);

        _lastYRotation = transform.eulerAngles.y;
    }
}
