using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

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

    [Header("UI")]
    [SerializeField] private Image left;
    [SerializeField] private Image right;
    [SerializeField] private Image top;
    [SerializeField] private Image bottom;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI reloadText;

    [Header("Settings")]
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float aimSmooth = 10f;

    private bool shootAction;
    private bool adsAction;
    private bool reloadAction;

    private const int maxAmmo = 7;
    private int currentAmmo;

    private bool isOperatingGun;
    private bool canShoot = true;
    private bool isReloading;

    private float reloadTimer = 1f;
    private float aimInaccuracy = 1f;
    private float lowerAimBound = 1f;

    private Vector3 recoilOffset;
    private Quaternion recoilRotation;

    private Vector3 localGunOffset = new Vector3(0.5f, 0.5f, 1.5f);

    private LayerMask shootLayers;

    void Awake()
    {
        shootAction = Input.GetKey(KeyCode.Mouse0);
        adsAction = Input.GetKey(KeyCode.Mouse1);
        reloadAction = Input.GetKey(KeyCode.R);

        currentAmmo = maxAmmo;
        //shootLayers = LayerMask.GetMask("geometry");
    }

    void Update()
    {
        shootAction = Input.GetKey(KeyCode.Mouse0);
        adsAction = Input.GetKey(KeyCode.Mouse1);
        reloadAction = Input.GetKey(KeyCode.R);

        isOperatingGun = true;
        
        HandleADS();
        HandleShooting();
        HandleReload();
        UpdateUI();
        UpdateGunTransform();
        UpdateCrosshair();
    }

    // -------------------------
    // Shooting
    // -------------------------

    void HandleShooting()
    {
        if (!isOperatingGun || isReloading) return;

        aimInaccuracy = Mathf.Lerp(aimInaccuracy, lowerAimBound, Time.deltaTime * 5f);
       
        if (shootAction && canShoot && currentAmmo > 0)
        {
            canShoot = false;
            currentAmmo--;
            SFX_gunshot.Play();

            float deviation = aimInaccuracy / 10f;
            Ray ray = Camera.main.ViewportPointToRay(
                new Vector3(
                    0.5f + Random.Range(-deviation, deviation),
                    0.5f + Random.Range(-deviation, deviation),
                    0f));

            if (Physics.Raycast(ray, out RaycastHit hit, 2000f, shootLayers))
            {
                hitParticle.transform.position = hit.point;
                hitParticle.transform.rotation = Quaternion.LookRotation(hit.normal);
                hitParticle.Play();
            }

            muzzleFlash.Play();

            recoilOffset = new Vector3(
                Random.Range(-0.1f, 0.1f),
                Random.Range(0.05f, 0.25f),
                Random.Range(-0.25f, 0.25f));

            recoilRotation = Quaternion.Euler(
                Random.Range(-60f, -20f),
                Random.Range(-5f, 5f),
                Random.Range(-3f, 3f));

            aimInaccuracy = 3f;
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
            ? new Vector3(0f, 0.85f, 1.6f)
            : new Vector3(0.5f, 0.5f, 1.5f);

        localGunOffset = Vector3.Lerp(localGunOffset, targetOffset, Time.deltaTime * aimSmooth);
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

        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, Time.deltaTime * 6f);
        recoilRotation = Quaternion.Lerp(recoilRotation, Quaternion.identity, Time.deltaTime * 10f);

        Vector3 worldOffset = Camera.main.transform.TransformDirection(localGunOffset);
        gun.transform.position = player.transform.position + worldOffset + recoilOffset;
        gun.transform.rotation = Camera.main.transform.rotation * recoilRotation;
        Vector3 angles = gun.transform.eulerAngles;
        //angles.y -= 90f;
        gun.transform.eulerAngles = angles;
    }

    // -------------------------
    // UI
    // -------------------------

    void UpdateUI()
    {
        ammoText.enabled = isOperatingGun;
        reloadText.enabled = isReloading;

        ammoText.text = $"{currentAmmo}/{maxAmmo}";
        reloadText.text = isReloading ? "[R...]" : "";
    }

    // -------------------------
    // Crosshair
    // -------------------------

    void UpdateCrosshair()
    {
        float offset = aimInaccuracy * 70f;

        left.rectTransform.anchoredPosition = new Vector2(-offset, 0);
        right.rectTransform.anchoredPosition = new Vector2(offset, 0);
        top.rectTransform.anchoredPosition = new Vector2(0, offset);
        bottom.rectTransform.anchoredPosition = new Vector2(0, -offset);
    }
}
