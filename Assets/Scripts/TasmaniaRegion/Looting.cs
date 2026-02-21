using System.Collections.Generic;
using UnityEngine;

public class Looting : MonoBehaviour
{
    
    public List<LootingSystem> possibleLoot; //loot that will spawn

    public List<(string, int)> GenerateLoot()
    {
        List<(string, int)> dropped = new List<(string, int)>();
        List<LootingSystem> commons = new List<LootingSystem>();


        //check list, if item = common put in commons, same thing for rare.
        foreach (LootingSystem item in possibleLoot)
        {
            if (item.rarity == Rarity.Common)
            {
                commons.Add(item);
            }
        }

        List<LootingSystem> rares = new List<LootingSystem>();

        foreach (LootingSystem item in possibleLoot)
        {
            if (item.rarity == Rarity.Rare)
            {
                rares.Add(item);
            }
        }
        //whether or not its rare or common
        //var commons = possibleLoot.Where(i => i.rarity == Rarity.Common).ToList(); //this one uses linq to make it neater, but i use the option above cause i can understand it better
        //var rares = possibleLoot.Where(i => i.rarity == Rarity.Rare).ToList(); //with lingq

        //ensure mostly common
        int commonCount = Random.Range(2, 5); //ensure within this rnage the amount of common items
        for (int i = 0; i < commonCount; i++)
        {
            LootingSystem item = commons[Random.Range(0, commons.Count)];
            int amount = Random.Range(item.minAmount, item.maxAmount + 1);
            dropped.Add((item.itemName, amount));
        }

       //30% will appear rare
        if (Random.value < 0.4f && rares.Count > 0)
        {
            LootingSystem rareItem = rares[Random.Range(0, rares.Count)];
            int amount = Random.Range(rareItem.minAmount, rareItem.maxAmount + 1);
            dropped.Add((rareItem.itemName, amount));
        }

        return dropped;
    }
}
