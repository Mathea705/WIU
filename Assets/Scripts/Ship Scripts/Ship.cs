using UnityEngine;

public class Ship : MonoBehaviour
{
    private Rigidbody  _playerRb;
    private Vector3    _lastPosition;
    private Quaternion _lastRotation;

    private void Start()
    {
        _lastPosition = transform.position;
        _lastRotation = transform.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            _playerRb = collision.gameObject.GetComponent<Rigidbody>();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            _playerRb = null;
    }

    private void FixedUpdate()
    {
        if (_playerRb != null)
        {
            Vector3 localPos = Quaternion.Inverse(_lastRotation) * (_playerRb.position - _lastPosition);
            _playerRb.MovePosition(transform.position + transform.rotation * localPos);
        }

        _lastPosition = transform.position;
        _lastRotation = transform.rotation;
    }
}
