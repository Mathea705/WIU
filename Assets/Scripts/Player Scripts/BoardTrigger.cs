using UnityEngine;

public class BoardTrigger : MonoBehaviour
{
    [SerializeField] private Transform   boardPoint;      
    [SerializeField] private PlayerController playerController;
    [SerializeField] private SwimController  swimController;
    [SerializeField] private GameObject    boardIcon;
    [SerializeField] private float  climbSpeed = 4f;

    private Rigidbody _rb;
    private bool _inRange;
    private bool _climbing;

    void Start()
    {
        _rb = playerController.GetComponent<Rigidbody>();
        if (boardIcon) boardIcon.SetActive(false);
    }

    void Update()
    {
        if (_climbing)
        {
    
            Vector3 target = boardPoint.position;
            _rb.MovePosition(Vector3.MoveTowards(_rb.position, target, climbSpeed * Time.deltaTime));

    
            if (Vector3.Distance(_rb.position, target) < 0.15f)
                FinishClimb();

            return;
        }

        if (_inRange && Input.GetKeyDown(KeyCode.E))
            StartClimb();
    }

    private void StartClimb()
    {
        _climbing = true;
        if (boardIcon) boardIcon.SetActive(false);


        playerController.enabled = false;
        swimController.enabled   = false;

        _rb.useGravity     = false;
        _rb.linearVelocity = Vector3.zero;
    }

    private void FinishClimb()
    {
        _climbing = false;
        _rb.MovePosition(boardPoint.position);


        swimController.ExitWater();
        playerController.enabled = true;
        swimController.enabled   = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _inRange = true;
        if (boardIcon) boardIcon.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _inRange = false;
        if (boardIcon) boardIcon.SetActive(false);
    }
}
