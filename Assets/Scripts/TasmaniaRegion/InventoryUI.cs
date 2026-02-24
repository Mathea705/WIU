using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel; //to hide it
    public Transform ContentPanel;
    public GameObject itemText; //item texts in the panel

    private bool isOpen = false;
    public static InventoryUI Instance;
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //toggle
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            RefreshUI();
        }
    }
    public void RefreshUI()
    {
        foreach (Transform child in ContentPanel)
            Destroy(child.gameObject);

        foreach (var entry in InventoryManager.instance.inventory)
        {
            GameObject obj = Instantiate(itemText, ContentPanel);
            obj.GetComponent<TMP_Text>().text = entry.lootedItem.itemName + " x" + entry.quantity + "\n";
        }
    }


}
