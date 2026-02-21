using UnityEngine;
using Unity.Cinemachine;

public class DriveTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineCamera    playerCamera;
    [SerializeField] private CinemachineCamera boatCamera;
    [SerializeField] private PlayerController  playerController;
    [SerializeField] private Transform  ship;
    [SerializeField] private BoxCollider hullBounds;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 60f;

    [SerializeField] private GameObject steerIcon;

    private Rigidbody playerRb;

    private bool _inRange;
    private bool _driving;

    void Start()
    {
        playerRb = playerController.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if ((_inRange || _driving) && Input.GetKeyDown(KeyCode.E))
        {
            SetDriving(!_driving);
            steerIcon.SetActive(false);
        }

        if (_driving)
            HandleSteering();
    }

    private void SetDriving(bool driving)
    {
        _driving                    = driving;
        playerController.enabled    = !driving;
        playerCamera.enabled        = !driving;
        boatCamera.gameObject.SetActive(driving);
        playerRb.linearVelocity = Vector3.zero;
    }

    private void HandleSteering()
    {
        float turn    = Input.GetAxis("Horizontal");
        float forward = Input.GetAxis("Vertical");

        ship.Rotate(Vector3.up, turn * turnSpeed * Time.deltaTime);

        if (forward != 0f)
        {
            Vector3 prev = ship.position;
            ship.position += ship.forward * (forward * moveSpeed * Time.deltaTime);

           
            if (forward > 0f && IsHullOverlappingWorld())
                ship.position = prev;
        }
    }

    private bool IsHullOverlappingWorld()
    {
        Vector3 worldCenter = hullBounds.transform.TransformPoint(hullBounds.center);
        Vector3 halfExtents = Vector3.Scale(hullBounds.size * 0.5f, hullBounds.transform.lossyScale);

        Collider[] hits = Physics.OverlapBox(worldCenter, halfExtents, hullBounds.transform.rotation,
                                             ~0, QueryTriggerInteraction.Ignore);
        foreach (Collider hit in hits)
        {
            if (hit.transform.IsChildOf(ship) || hit.transform == ship) continue;
            if (hit.CompareTag("Player")) continue;
            return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            steerIcon.SetActive(true);
            _inRange = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
         if (other.CompareTag("Player") && !_driving)
        {
            steerIcon.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            steerIcon.SetActive(false);
            _inRange = false;
        }
    }
}
