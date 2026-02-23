using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform ContentPanel;
    public GameObject itemText; //item texts in the panel
   
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
