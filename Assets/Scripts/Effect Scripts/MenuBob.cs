using UnityEngine;

public class MenuBob : MonoBehaviour
{
 
    [SerializeField] private float heaveAmount = 0.3f;
    [SerializeField] private float heaveSpeed  = 0.8f;
    [SerializeField] private float swayAmount  = 0.1f;
    [SerializeField] private float swaySpeed   = 0.5f;

    [SerializeField] private float rollAmount  = 4f;
    [SerializeField] private float rollSpeed   = 0.6f;
    [SerializeField] private float pitchAmount = 2f;
    [SerializeField] private float pitchSpeed  = 0.4f;
    [SerializeField] private float yawAmount   = 1f;
    [SerializeField] private float yawSpeed    = 0.3f;

    private Vector3    _startPos;
    private Quaternion _startRot;

    private void Start()
    {
        _startPos = transform.localPosition;
        _startRot = transform.localRotation;
    }

    private void Update()
    {
        float time = Time.time;

        Vector3 posOffset = new Vector3(
            Mathf.Sin(time * swaySpeed  + 1.3f) * swayAmount,
            Mathf.Sin(time * heaveSpeed + 0.0f) * heaveAmount,
            0f
        );

        Vector3 rotOffset = new Vector3(
            Mathf.Sin(time * pitchSpeed + 2.1f) * pitchAmount,
            Mathf.Sin(time * yawSpeed   + 0.7f) * yawAmount,
            Mathf.Sin(time * rollSpeed  + 4.5f) * rollAmount
        );

        transform.localPosition = _startPos + posOffset;
        transform.localRotation = _startRot * Quaternion.Euler(rotOffset);
    }
}
