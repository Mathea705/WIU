using UnityEngine;


public class SwimController : MonoBehaviour
{

    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform        lookPivot;

    [SerializeField] private GameObject splashParticlePrefab;
    [SerializeField] private GameObject splashImagePrefab;

    [SerializeField] private float swimSpeed     = 3.5f;
    [SerializeField] private float buoyancy      = 2.0f;
    [SerializeField] private float mouseSensitivity = 2f;
    // [SerializeField] private GameObject underwaterOverlay;

    public bool  IsSwimming   => _isSwimming;
    public bool  IsUnderwater => _isUnderwater;
    public float WaterDepth   => _isUnderwater ? Mathf.Max(0f, _waterSurfaceY - _rb.position.y) : 0f;

    private Rigidbody _rb;
    private bool  _isSwimming;
    private bool  _isUnderwater;
    private float _waterSurfaceY;
    private float _xRotation;  

    void Awake()
    {
        _rb = playerController.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!_isSwimming) return;

        HandleLook();
        SetUnderwater(lookPivot.position.y < _waterSurfaceY);
    }

    void FixedUpdate()
    {
        if (!_isSwimming) return;
        HandleSwimMovement();
    }


    public void EnterWater(float surfaceY)
    {
        if (_isSwimming) return;
        _waterSurfaceY   = surfaceY;
        _isSwimming   = true;
        playerController.enabled = false;
        _rb.useGravity  = false;
        _rb.linearVelocity  = Vector3.zero;
        float e = lookPivot.localEulerAngles.x;
        _xRotation = e > 180f ? e - 360f : e;
        SpawnSplash(surfaceY);
    }

    public void ExitWater()
    {
        _isSwimming  = false;
        playerController.enabled = true;
        _rb.useGravity  = true;
        SetUnderwater(false);
        SpawnSplash(_waterSurfaceY);
    }

    private void SpawnSplash(float surfaceY)
    {
        Vector3 pos = new(_rb.position.x, surfaceY, _rb.position.z);

    
            GameObject splash = Instantiate(splashParticlePrefab, pos, Quaternion.identity);
            Destroy(splash, 1f);
        

       
            GameObject image = Instantiate(splashImagePrefab, pos, Quaternion.identity);
            Destroy(image, 2f);
        
    }


    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        _xRotation -= mouseY;
        _xRotation  = Mathf.Clamp(_xRotation, -90f, 90f);
        lookPivot.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        playerController.transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleSwimMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

     
        Vector3 move = lookPivot.forward * v + lookPivot.right * h;

        if (Input.GetKey(KeyCode.Space))        move.y += 1f;
        if (Input.GetKey(KeyCode.LeftControl))  move.y -= 1f;

        if (move.sqrMagnitude > 1f) move.Normalize();

        _rb.linearVelocity = Vector3.Lerp(
            _rb.linearVelocity, move * swimSpeed, Time.fixedDeltaTime * 6f);

        float depth = _waterSurfaceY - _rb.position.y;
        if (depth > 0.2f)
            _rb.AddForce(Vector3.up * Mathf.Min(depth, 1.5f) * buoyancy,
                         ForceMode.Acceleration);
    }

    private void SetUnderwater(bool value)
    {
        if (_isUnderwater == value) return;
        _isUnderwater = value;

            // underwaterOverlay.SetActive(value);
    }
}
