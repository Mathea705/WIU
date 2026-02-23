using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class chestLogic : MonoBehaviour
{
    private bool opened = false;
    private Looting loot;

    private List<(LootingItem, int)> storedLoot; //store the loot


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        loot = GetComponent<Looting>();
    }
    public void OpenChest()
    {
        if (opened)
            return;

        opened = true;
        var generatedLoot = loot.GenerateLoot();
        InventoryManager.instance.AddLootToInventory(generatedLoot);
        FindAnyObjectByType<InventoryUI>().RefreshUI(); //find ui and will display items
        //foreach (var item in storedLoot) //show loot in chest
        //{
        //    Debug.Log("Got: " + item.Item1 + " x" + item.Item2);
        //}

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    //for ui
    //public List<(LootingItem, int)> GetLoot()
    //{
    //    return storedLoot;
    //}
}
