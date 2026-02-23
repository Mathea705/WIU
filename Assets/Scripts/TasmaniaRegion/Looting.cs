using System.Collections.Generic;
using UnityEngine;

public class Looting : MonoBehaviour
{
    
    public List<LootingItem> possibleLoot; //loot that will spawn

    public List<(LootingItem, int)> GenerateLoot()
    {
        List<(LootingItem, int)> dropped = new List<(LootingItem, int)>();
        List<LootingItem> commons = new List<LootingItem>();


        //check list, if item = common put in commons, same thing for rare.
        foreach (LootingItem item in possibleLoot)
        {
            if (item.rarity == Rarity.Common)
            {
                commons.Add(item);
            }
        }

        List<LootingItem> rares = new List<LootingItem>();

        foreach (LootingItem item in possibleLoot)
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

        commonCount = Mathf.Min(commonCount, commons.Count);
        for (int i = 0; i < commonCount; i++)
        {
            int randomIndex = Random.Range(0, commons.Count);

            LootingItem item = commons[/*Random.Range(0, commons.Count)*/randomIndex];
            //commons.RemoveAt(Random.Range(0, commons.Count));
            commons.RemoveAt(randomIndex);
            int amount = Random.Range(item.minAmount, item.maxAmount + 1);
            dropped.Add((item, amount));
        }

       //30% will appear rare //well now 40%
        if (Random.value < 0.4f && rares.Count > 0)
        {
            LootingItem rareItem = rares[Random.Range(0, rares.Count)];
            //rares.RemoveAt(Random.Range(0, rares.Count)); //prevent duplication
            int amount = Random.Range(rareItem.minAmount, rareItem.maxAmount + 1);
            dropped.Add((rareItem, amount));
        }

        return dropped;
    }
}
