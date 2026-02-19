using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runMultiplier = 2f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 2f;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private LayerMask groundMask;

    [Header("Head Bob")]
    [SerializeField] private float bobFrequency = 2f;
    [SerializeField] private float bobAmplitude = 0.05f;

    [SerializeField] private Transform lookPivot;

    private Rigidbody rb;
    private float xRotation = 0f;
    private bool isGrounded;

    private float bobTimer = 0f;
    private float defaultCameraY;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        defaultCameraY = lookPivot.transform.localPosition.y;
    }

    void Update()
    {
        HandleLook();
        HandleJump();
        HandleHeadBob();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        lookPivot.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleJump()
    {
        
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2 + 0.2f, groundMask);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical   = Input.GetAxis("Vertical");

        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? runMultiplier : 1f);

        Vector3 movement = transform.right * horizontal + transform.forward * vertical;
        rb.MovePosition(rb.position + movement * (speed * Time.fixedDeltaTime));
    }

    private void HandleHeadBob()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical   = Input.GetAxis("Vertical");
        bool isMoving    = (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f) && isGrounded;

        if (isMoving)
        {
            float bobSpeed = bobFrequency * (Input.GetKey(KeyCode.LeftShift) ? runMultiplier : 1f);
            bobTimer += Time.deltaTime * bobSpeed;

            Vector3 pos = lookPivot.transform.localPosition;
            pos.y = defaultCameraY + Mathf.Sin(bobTimer) * bobAmplitude;
            lookPivot.transform.localPosition = pos;
        }
        else
        {
            bobTimer = 0f;
            Vector3 pos = lookPivot.transform.localPosition;
            pos.y = Mathf.Lerp(pos.y, defaultCameraY, Time.deltaTime * 5f);
            lookPivot.transform.localPosition = pos;
        }
    }
}
