using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Looting : MonoBehaviour
{
    public List<LootingSystem> possibleLoot; //loot that will spawn

    public List<(string, int)> GenerateLoot()
    {
        List<(string, int)> dropped = new List<(string, int)>();

        var commons = possibleLoot.Where(i => i.rarity == Rarity.Common).ToList();
        var rares = possibleLoot.Where(i => i.rarity == Rarity.Rare).ToList();

        //ensure mostly common
        int commonCount = Random.Range(2, 5);
        for (int i = 0; i < commonCount; i++)
        {
            LootingSystem item = commons[Random.Range(0, commons.Count)];
            int amount = Random.Range(item.minAmount, item.maxAmount + 1);
            dropped.Add((item.itemName, amount));
        }

       //30% will appear rare
        if (Random.value < 0.3f && rares.Count > 0)
        {
            LootingSystem rareItem = rares[Random.Range(0, rares.Count)];
            int amount = Random.Range(rareItem.minAmount, rareItem.maxAmount + 1);
            dropped.Add((rareItem.itemName, amount));
        }

        return dropped;
    }
}
