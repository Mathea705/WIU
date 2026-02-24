using NUnit.Framework;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public AurumManager aurum;
    public List<InventoryAdding> inventory = new List<InventoryAdding>();   //looting inventory for keeping purposes
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void AddLootToInventory(List<(LootingItem, int)> loot)
    {
        foreach (var drop in loot)
        {
            AddItem(drop.Item1, drop.Item2);
        }
    }
    public void AddItem(LootingItem item, int amount)
    {
        InventoryAdding existing = inventory.Find(i => i.lootedItem == item); //checks if item alrdy in the inventory

        if (existing != null)
        {
            existing.quantity += amount;
            
          
        }
        else
        {
            inventory.Add(new InventoryAdding(item, amount));  //add
        }
    }
    public void SellAll()
    {
        int totalValue = 0;

        foreach (InventoryAdding entry in inventory) //sell ALL ITEMS
        {
            int value = entry.lootedItem.sellValue * entry.quantity;
            totalValue += value;
        }

        FindAnyObjectByType<AurumManager>().AddAurum(totalValue); //add aurum using aurumManager;

        Debug.Log("Sold everything for: " + totalValue + " Aurum");
        Debug.Log("Total Aurum: " + aurum); //why not

        inventory.Clear();

     
        if (InventoryUI.Instance != null)
            InventoryUI.Instance.RefreshUI();
    }
    //when selling, nvm cancel this
    //private void RemoveItem(LootingItem item, int amount)
    //{
    //    InventoryAdding existing = inventory.Find(i => i.lootedItem == item);

    //    if (existing != null)
    //    {
    //        existing.quantity -= amount;

    //        if (existing.quantity <= 0)
    //            inventory.Remove(existing);
    //    }
    //}
    // Update is called once per frame
    void Update()
    {
        
    }
}
