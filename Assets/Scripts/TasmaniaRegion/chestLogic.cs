using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class chestLogic : MonoBehaviour
{
    private bool opened = false;
    private Looting loot;

    private List<(string, int)> storedLoot; //store the loot


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
        storedLoot = loot.GenerateLoot();
        foreach (var item in storedLoot) //show loot in chest
        {
            Debug.Log("Got: " + item.Item1 + " x" + item.Item2);
        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    //for ui
    public List<(string, int)> GetLoot()
    {
        return storedLoot;
    }
}
