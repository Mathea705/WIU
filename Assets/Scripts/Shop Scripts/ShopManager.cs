using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private AurumManager    aurumManager;
    [SerializeField] private HealthSystem    healthSystem;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private StaminaSystem   staminaSystem;

    [SerializeField] private GameObject gunsPanel;
    [SerializeField] private GameObject itemsPanel;
    [SerializeField] private GameObject sellPanel;

    [SerializeField] private Transform gunsContent;
    [SerializeField] private Transform itemsContent;
    [SerializeField] private Transform sellContent;

    [SerializeField] private ShopItemData[] gunItems;
    [SerializeField] private GameObject[]  sceneGuns;
    [SerializeField] private ShopItemData[] shopItems;

    [SerializeField] private ShopSlot slotPrefab;

    private string[] _gunNames;

    private void Awake()
    {
        if (sceneGuns != null)
        {
            _gunNames = new string[sceneGuns.Length];
            for (int i = 0; i < sceneGuns.Length; i++)
                if (sceneGuns[i] != null) _gunNames[i] = sceneGuns[i].name;
        }
    }

    private void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => RebindRefs();

    private static GameObject FindPersistent(string objName)
    {
        foreach (var t in FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (t.name == objName && t.gameObject.scene.name == "DontDestroyOnLoad")
                return t.gameObject;
        return null;
    }

    private void RebindRefs()
    {
        foreach (var am in FindObjectsByType<AurumManager>(FindObjectsSortMode.None))
            if (am.gameObject.scene.name == "DontDestroyOnLoad") { aurumManager = am; break; }

        foreach (var pc in FindObjectsByType<PlayerController>(FindObjectsSortMode.None))
            if (pc.gameObject.scene.name == "DontDestroyOnLoad") { playerController = pc; break; }

        foreach (var hs in FindObjectsByType<HealthSystem>(FindObjectsSortMode.None))
            if (hs.gameObject.scene.name == "DontDestroyOnLoad") { healthSystem = hs; break; }

        foreach (var ss in FindObjectsByType<StaminaSystem>(FindObjectsSortMode.None))
            if (ss.gameObject.scene.name == "DontDestroyOnLoad") { staminaSystem = ss; break; }

        if (_gunNames != null && sceneGuns != null)
            for (int i = 0; i < sceneGuns.Length; i++)
                if (_gunNames[i] != null)
                {
                    GameObject found = FindPersistent(_gunNames[i]);
                    if (found != null) sceneGuns[i] = found;
                }
    }

    private void Start()
    {
        RebindRefs();
        RefreshBuy(null);
        Populate(itemsContent, shopItems);
        ShowGuns();
    }

    public void RefreshBuy(List<GameObject> ownedGuns)
    {
        foreach (Transform child in gunsContent)
            Destroy(child.gameObject);

        for (int i = 0; i < gunItems.Length; i++)
        {
            ShopSlot slot = Instantiate(slotPrefab, gunsContent);
            GameObject gun = (sceneGuns != null && i < sceneGuns.Length) ? sceneGuns[i] : null;
            bool alreadyOwned = gun != null && ownedGuns != null && ownedGuns.Contains(gun);
            slot.Setup(gunItems[i], aurumManager, gun, alreadyOwned);
        }
    }

    private void Populate(Transform content, ShopItemData[] items)
    {
        foreach (ShopItemData data in items)
        {
            ShopSlot slot = Instantiate(slotPrefab, content);
            slot.Setup(data, aurumManager, null, false, healthSystem, playerController, staminaSystem);
        }
    }

    public void RefreshSell(List<GameObject> ownedGuns)
    {
        foreach (Transform child in sellContent)
            Destroy(child.gameObject);

        foreach (GameObject ownedGun in ownedGuns)
        {
            for (int i = 0; i < sceneGuns.Length; i++)
            {
                if (sceneGuns[i] == ownedGun && i < gunItems.Length)
                {
                    ShopSlot slot = Instantiate(slotPrefab, sellContent);
                    slot.SetupSell(gunItems[i], aurumManager, ownedGun);
                    break;
                }
            }
        }
    }

    public void ShowGuns() => ShowTab(gunsPanel);
    public void ShowItems() => ShowTab(itemsPanel);
    public void ShowSell() => ShowTab(sellPanel);

    private void ShowTab(GameObject active)
    {
        gunsPanel .SetActive(active == gunsPanel);
        itemsPanel.SetActive(active == itemsPanel);
        sellPanel .SetActive(active == sellPanel);
    }
}
