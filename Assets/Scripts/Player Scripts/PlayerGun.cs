using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class GunController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject gun;

    [Header("Gun FX")]
    [SerializeField] private ParticleSystem hitParticle;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioSource SFX_gunshot;
    [SerializeField] private AudioSource SFX_reload;

    [Header("Gun Lerp Stuff")]
    [SerializeField] private float movementSwayAmount = 10f;
    [SerializeField] private float mouseSwayAmount = 2f;
    [SerializeField] private float swaySmooth = 8f;
    [SerializeField] private float LERP_ADS = 10f;
    [SerializeField] private float LERP_GUNPOS = 20f;
    [SerializeField] private float LERP_AIMINACCURACY = 5f;
    [SerializeField] private float LERP_INPUTSWAY = 10f;
    [SerializeField] private float LERP_RECOILTRANSFORM = 10f;
    [SerializeField] private float LERP_RECOILROT = 5f;

    private Vector2 currentSway;
    private Vector2 swayVelocity;
    private Vector2 mouseDelta;

    private Rigidbody playerRb;

    [Header("UI")]
    [SerializeField] private Image left;
    [SerializeField] private Image right;
    [SerializeField] private Image top;
    [SerializeField] private Image bottom;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI reloadText;

    [Header("Settings")]
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float MOVEINACCURACY = 0.1f;
    [SerializeField] private float CROSSHAIROFFSET = 70f;
    [SerializeField] private float HIPFIREINACCURACY = 0.5f;
    [SerializeField] private float ADSINACCURACY = -0.15f;
    [SerializeField] private float SHOOTINACCURACYADS = 1.5f;
    [SerializeField] private float SHOOTINACCURACYHIP = 3f;
    [SerializeField] private Vector3 adsPosRecoilLowerBound = new Vector3(0f, 0.05f, -0.05f);
    [SerializeField] private Vector3 adsPosRecoilUpperBound = new Vector3(0f, 0.1f, 0.05f);
    [SerializeField] private Vector3 adsRotRecoilLowerBound = new Vector3(-20f, -3f, -1f);
    [SerializeField] private Vector3 adsRotRecoilUpperBound = new Vector3(-10f, 3f, 1f);
    [SerializeField] private Vector3 hipPosRecoilLowerBound = new Vector3(0f, 0.05f, -0.25f);
    [SerializeField] private Vector3 hipPosRecoilUpperBound = new Vector3(0f, 0.25f, 0.25f);
    [SerializeField] private Vector3 hipRotRecoilLowerBound = new Vector3(-60f, -5f, -3f);
    [SerializeField] private Vector3 hipRotRecoilUpperBound = new Vector3(-20f, 5f, 3f);

    [SerializeField] private int maxAmmo = 20;

    [SerializeField] private bool isOperatingGun;
    [SerializeField] private bool isFullAuto;
    [SerializeField] private float firerateCooldown;

    private float shootCooldown;

    private bool shootAction;
    private bool adsAction;
    private bool reloadAction;

    private int currentAmmo;

    private bool canShoot = true;
    private bool isReloading;

    private float reloadTimer = 1f;
    private float aimInaccuracy = 1f;
    private float lowerAimBound = 1f;

    private Vector3 recoilOffset;
    private Quaternion recoilRotation;

    private Camera playerCam;

    private Vector3 localGunOffset = new Vector3(0.5f, 0.5f, 1.5f);

    [SerializeField] private Vector3 OFFSETDEFAULT = new Vector3(0.5f, 0.5f, 1.5f);
    [SerializeField] private Vector3 OFFSETADS = new Vector3(0f, 0.85f, 1.6f);

    private LayerMask shootLayers;

    private Vector2 playerInputVector;
    private Vector2 inputSway;

    void Awake()
    {
        shootAction = Input.GetKey(KeyCode.Mouse0);
        adsAction = Input.GetKey(KeyCode.Mouse1);
        reloadAction = Input.GetKey(KeyCode.R);

        currentAmmo = maxAmmo;

        playerRb = player.GetComponent<Rigidbody>();
        //shootLayers = LayerMask.GetMask("geometry");

        playerCam = Camera.main;
    }

    void Update()
    {
        shootAction = Input.GetKey(KeyCode.Mouse0);
        adsAction = Input.GetKey(KeyCode.Mouse1);
        reloadAction = Input.GetKey(KeyCode.R);

        HandleADS();
        HandleShooting();
        HandleReload();
        HandleRecoil();
        UpdateUI();
        UpdateGunTransform();
        UpdateCrosshair();

        shootCooldown -= Time.deltaTime;
        if (shootCooldown < 0f)
        {
            shootCooldown = 0f;
        }
    }

    // -------------------------
    // Shooting
    // -------------------------

    void HandleShooting()
    {
        aimInaccuracy += Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.W) ? MOVEINACCURACY : 0;

        lowerAimBound = adsAction ? ADSINACCURACY : HIPFIREINACCURACY;

        aimInaccuracy = Mathf.Lerp(aimInaccuracy, lowerAimBound, Time.deltaTime * LERP_AIMINACCURACY);

        if (!isOperatingGun || isReloading) return;

        if (shootAction && canShoot && currentAmmo > 0 && shootCooldown <= 0f)
        {
            if (!isFullAuto)
            {
                canShoot = false;
            }
            currentAmmo--;
            shootCooldown = firerateCooldown;

            SFX_gunshot.Play();

            float deviation = aimInaccuracy / 10f;
            Ray ray = playerCam.ViewportPointToRay(
                new Vector3(
                    0.5f + Random.Range(-deviation, deviation),
                    0.5f + Random.Range(-deviation, deviation),
                    0f));

            if (Physics.Raycast(ray, out RaycastHit hit, 2000f))//, shootLayers))
            {
                //hitParticle.transform.position = hit.point;
                //hitParticle.transform.rotation = Quaternion.LookRotation(hit.normal);
                hitParticle.Play();
            }
            
            muzzleFlash.Play();

            if (adsAction)
            {
                recoilOffset = new Vector3(
                    Random.Range(adsPosRecoilLowerBound.x, adsPosRecoilUpperBound.x),
                    Random.Range(adsPosRecoilLowerBound.y, adsPosRecoilUpperBound.y),
                    Random.Range(adsPosRecoilLowerBound.z, adsPosRecoilUpperBound.z));

                recoilRotation = Quaternion.Euler(
                    Random.Range(adsRotRecoilLowerBound.x, adsRotRecoilUpperBound.x),
                    Random.Range(adsRotRecoilLowerBound.y, adsRotRecoilUpperBound.y),
                    Random.Range(adsRotRecoilLowerBound.z, adsRotRecoilUpperBound.z));
                aimInaccuracy = SHOOTINACCURACYADS;
            }
            else
            {
                recoilOffset = new Vector3(
                    Random.Range(hipPosRecoilLowerBound.x, hipPosRecoilUpperBound.x),
                    Random.Range(hipPosRecoilLowerBound.y, hipPosRecoilUpperBound.y),
                    Random.Range(hipPosRecoilLowerBound.z, hipPosRecoilUpperBound.z));

                recoilRotation = Quaternion.Euler(
                    Random.Range(hipRotRecoilLowerBound.x, hipRotRecoilUpperBound.x),
                    Random.Range(hipRotRecoilLowerBound.y, hipRotRecoilUpperBound.y),
                    Random.Range(hipRotRecoilLowerBound.z, hipRotRecoilUpperBound.z));
                aimInaccuracy = SHOOTINACCURACYHIP;
            }
        }

        if (!shootAction)
            canShoot = true;
    }

    // -------------------------
    // ADS
    // -------------------------

    void HandleADS()
    {
        Vector3 targetOffset = adsAction
            ? OFFSETADS
            : OFFSETDEFAULT;

        localGunOffset = Vector3.Lerp(localGunOffset, targetOffset, Time.deltaTime * LERP_ADS);
    }

    // -------------------------
    // Reload
    // -------------------------

    void HandleReload()
    {
        if (reloadAction && currentAmmo < maxAmmo && !isReloading)
        {
            isReloading = true;
            SFX_reload.Play();
            reloadTimer = 1f;
        }

        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;

            if (reloadTimer <= 0f)
            {
                currentAmmo = maxAmmo;
                isReloading = false;
            }
        }
    }

    // -------------------------
    // Gun Positioning
    // -------------------------

    void UpdateGunTransform()
    {
        if (!isOperatingGun) return;

        playerInputVector.x = (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) ? 1 : !Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D) ? -1 : 0) * 50;
        playerInputVector.y = (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W) ? 1 : !Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.W) ? -1 : 0) * 50;

        recoilRotation = Quaternion.Lerp(recoilRotation, Quaternion.identity, Time.deltaTime * 10f);

        Vector3 worldOffset = playerCam.transform.TransformDirection(localGunOffset);
        gun.transform.position = Vector3.Lerp(gun.transform.position, playerCam.transform.position + worldOffset + recoilOffset, Time.deltaTime * LERP_GUNPOS);
        gun.transform.rotation = Quaternion.Lerp(gun.transform.rotation, playerCam.transform.rotation, Time.deltaTime * LERP_GUNPOS) * recoilRotation;

        inputSway = Vector2.Lerp(inputSway, playerInputVector, Time.deltaTime * LERP_INPUTSWAY);

        Quaternion finalRotation =
            playerCam.transform.rotation *
            Quaternion.Euler(inputSway.y, inputSway.x, 0f) *
            recoilRotation;

        gun.transform.rotation = Quaternion.Lerp(
            gun.transform.rotation,
            finalRotation,
            Time.deltaTime * 10f);

    }

    // -------------------------
    // Gun Recoil
    // -------------------------
    
    void HandleRecoil()
    {
        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, Time.deltaTime * LERP_RECOILTRANSFORM);
        recoilRotation = Quaternion.Lerp(recoilRotation, Quaternion.identity, Time.deltaTime * LERP_RECOILROT);
    }


    // -------------------------
    // UI
    // -------------------------

    void UpdateUI()
    {
        ammoText.enabled = isOperatingGun;
        reloadText.enabled = isReloading || (isOperatingGun && currentAmmo <= 0);

        ammoText.color = currentAmmo <= Mathf.Ceil(maxAmmo / 3.5f) ? Color.red : Color.black;
        reloadText.color = currentAmmo <= Mathf.Ceil(maxAmmo / 3.5f) ? Color.red : Color.black;

        ammoText.text = $"{currentAmmo}/{maxAmmo}";
        reloadText.text = isReloading ? "[R...]" : currentAmmo <= 0 ? "[R!]" : "";
    }

    // -------------------------
    // Crosshair
    // -------------------------

    void UpdateCrosshair()
    {
        float offset = aimInaccuracy * CROSSHAIROFFSET;
        
        left.rectTransform.anchoredPosition = new Vector2(-offset, 0);
        right.rectTransform.anchoredPosition = new Vector2(offset, 0);
        top.rectTransform.anchoredPosition = new Vector2(0, offset);
        bottom.rectTransform.anchoredPosition = new Vector2(0, -offset);

        top.enabled = !adsAction;
        //bottom.enabled = !adsAction;
    }
}
