using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using TMPro;
using UnityEngine.SceneManagement;

public class ShopTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineCamera shopCamera;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject shopIcon;
    [SerializeField] private GameObject[] hideOnShopOpen;

    [SerializeField] private GameObject  dialogueBox;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private string  firstLine   = "Welcome, traveller!";
    [SerializeField] private string  repeatLine  = "See anything you like?";
    [SerializeField] private float  typeSpeed = 0.05f;

    [SerializeField] private GameObject shopContent;
    [SerializeField] private float cameraRotateDuration = 0.5f;
    [SerializeField] private AurumManager AurumManager;
    [SerializeField] private TMP_Text     shopAurumText;

    [SerializeField] private GameObject   gunCanvas;
    [SerializeField] private GameObject[] allGuns;
    [SerializeField] private ShopManager  shopManager;

    private bool _inRange;
    private bool _shopOpen;
    private static bool _hasVisited;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() => _hasVisited = false;
    private Quaternion _shopCameraStartRotation;
    private readonly List<GameObject> _gunsActiveBeforeShop = new List<GameObject>();
    private string[] _hideNames;
    private string[] _gunNames;
    private string   _gunCanvasName;

    private void Awake()
    {
        // Cache names before sceneLoaded cleanup destroys scene-native duplicates.
        _hideNames = new string[hideOnShopOpen.Length];
        for (int i = 0; i < hideOnShopOpen.Length; i++)
            if (hideOnShopOpen[i] != null)
                _hideNames[i] = hideOnShopOpen[i].name;

        if (allGuns != null)
        {
            _gunNames = new string[allGuns.Length];
            for (int i = 0; i < allGuns.Length; i++)
                if (allGuns[i] != null)
                    _gunNames[i] = allGuns[i].name;
        }

        if (gunCanvas != null) _gunCanvasName = gunCanvas.name;
    }

    private void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => RebindRefs();

    // Search only the DontDestroyOnLoad scene by name, including inactive objects.
    private static GameObject FindPersistent(string objName)
    {
        foreach (var t in FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (t.name == objName && t.gameObject.scene.name == "DontDestroyOnLoad")
                return t.gameObject;
        return null;
    }

    private void RebindRefs()
    {
        // Always reassign — no null check — so deferred-Destroy timing never matters.

        // PlayerController: find persistent player by scene name
        foreach (var pc in FindObjectsByType<PlayerController>(FindObjectsSortMode.None))
            if (pc.gameObject.scene.name == "DontDestroyOnLoad") { playerController = pc; break; }

        // hideOnShopOpen: prefer persistent, fall back to scene-native (some panels may not persist)
        if (_hideNames != null)
            for (int i = 0; i < hideOnShopOpen.Length; i++)
                if (_hideNames[i] != null)
                {
                    GameObject found = FindPersistent(_hideNames[i]);
                    if (found == null) found = GameObject.Find(_hideNames[i]);
                    if (found != null) hideOnShopOpen[i] = found;
                }

        // allGuns: live on the persistent player — only look in DontDestroyOnLoad
        if (_gunNames != null && allGuns != null)
            for (int i = 0; i < allGuns.Length; i++)
                if (_gunNames[i] != null)
                {
                    GameObject found = FindPersistent(_gunNames[i]);
                    if (found != null) allGuns[i] = found;
                }

        // gunCanvas: persistent canvas
        if (_gunCanvasName != null)
        {
            GameObject found = FindPersistent(_gunCanvasName);
            if (found == null) found = GameObject.Find(_gunCanvasName);
            if (found != null) gunCanvas = found;
        }

        // AurumManager: find the persistent one by scene name
        foreach (var am in FindObjectsByType<AurumManager>(FindObjectsSortMode.None))
            if (am.gameObject.scene.name == "DontDestroyOnLoad") { AurumManager = am; break; }
    }

    private void Start()
    {
        _shopCameraStartRotation = shopCamera.transform.localRotation;
        RebindRefs();
    }

    private void Update()
    {
        if ((_inRange || _shopOpen) && Input.GetKeyDown(KeyCode.E))
        {
            if (!_shopOpen) OpenShop();
            else  CloseShop();
        }
    }

    private void OpenShop()
    {
        _shopOpen = true;
        shopIcon.SetActive(false);
        shopCamera.gameObject.SetActive(true);
        playerController.enabled = false;
        foreach (GameObject panel in hideOnShopOpen)
            if (panel != null) panel.SetActive(false);

        _gunsActiveBeforeShop.Clear();
        ShopSlot.GunOwned = false;
        if (allGuns != null)
            foreach (GameObject gun in allGuns)
                if (gun != null && gun.activeSelf)
                {
                    ShopSlot.GunOwned = true;
                    _gunsActiveBeforeShop.Add(gun);
                    gun.SetActive(false);
                }

        if (shopManager != null)
        {
            shopManager.RefreshBuy(_gunsActiveBeforeShop);
            shopManager.RefreshSell(_gunsActiveBeforeShop);
        }

        if (AurumManager != null && shopAurumText != null)
            AurumManager.SetShopDisplay(shopAurumText);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
        dialogueBox.SetActive(true);
        shopContent.SetActive(false);
        StartCoroutine(PlayDialogue());
    }

    private void CloseShop()
    {
        _shopOpen = false;
        StopAllCoroutines();
        shopCamera.gameObject.SetActive(false);
        shopCamera.transform.localRotation = _shopCameraStartRotation;
        playerController.enabled = true;
        foreach (GameObject panel in hideOnShopOpen)
            if (panel != null) panel.SetActive(true);

        foreach (GameObject gun in _gunsActiveBeforeShop)
            if (gun != null && !ShopSlot.SoldGuns.Contains(gun)) gun.SetActive(true);
        ShopSlot.SoldGuns.Clear();

        foreach (GameObject gun in ShopSlot.PendingGuns)
            if (gun != null) gun.SetActive(true);
        ShopSlot.PendingGuns.Clear();

        if (gunCanvas != null)
        {
            bool ownsGun = false;
            if (allGuns != null)
                foreach (GameObject gun in allGuns)
                    if (gun != null && gun.activeSelf) { ownsGun = true; break; }
            gunCanvas.SetActive(ownsGun);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
        dialogueBox.SetActive(false);
        shopContent.SetActive(false);
    }

    private IEnumerator PlayDialogue()
    {
        dialogueText.text = "";
        var typeWait = new WaitForSeconds(typeSpeed);
        string line = _hasVisited ? repeatLine : firstLine;
         if (!_hasVisited)
        {
             AurumManager.AddAurum(200);
        }
        
        _hasVisited = true;

       
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return typeWait;
        }
        yield return new WaitForSeconds(0.5f);
        // yield return StartCoroutine(RotateCamera());
        dialogueBox.SetActive(false);
        shopContent.SetActive(true);
    }

    // private IEnumerator RotateCamera()
    // {
    //     Quaternion startRot = shopCamera.transform.localRotation;
    //     Quaternion endRot   = startRot * Quaternion.Euler(0f, 30f, 0f);
    //     float elapsed = 0f;
    //     while (elapsed < cameraRotateDuration)
    //     {
    //         elapsed += Time.deltaTime;
    //         shopCamera.transform.localRotation = Quaternion.Slerp(startRot, endRot, elapsed / cameraRotateDuration);
    //         yield return null;
    //     }
    //     shopCamera.transform.localRotation = endRot;
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopIcon.SetActive(true);
            _inRange = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !_shopOpen)
            shopIcon.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopIcon.SetActive(false);
            _inRange = false;
        }
    }
}
