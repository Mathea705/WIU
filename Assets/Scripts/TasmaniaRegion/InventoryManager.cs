using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

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
    //when selling
    private void RemoveItem(LootingItem item, int amount)
    {
        InventoryAdding existing = inventory.Find(i => i.lootedItem == item);

        if (existing != null)
        {
            existing.quantity -= amount;

            if (existing.quantity <= 0)
                inventory.Remove(existing);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
