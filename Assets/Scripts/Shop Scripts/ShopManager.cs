using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private AurumManager aurumManager;

    [SerializeField] private GameObject gunsPanel;
    [SerializeField] private GameObject itemsPanel;
    [SerializeField] private GameObject sellPanel;

    [SerializeField] private Transform gunsContent;
    [SerializeField] private Transform itemsContent;

    [SerializeField] private ShopItemData[] gunItems;
    [SerializeField] private ShopItemData[] shopItems;

    [SerializeField] private ShopSlot slotPrefab;

    private void Start()
    {
        Populate(gunsContent, gunItems);
        Populate(itemsContent, shopItems);
        ShowGuns();
    }

    private void Populate(Transform content, ShopItemData[] items)
    {
        foreach (ShopItemData data in items)
        {
            ShopSlot slot = Instantiate(slotPrefab, content);
            slot.Setup(data, aurumManager);
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
