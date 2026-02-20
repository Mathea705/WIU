using UnityEngine;
public enum Rarity
{
    Common,
    Rare
}
public class LootingSystem
{
    public string itemName;
    public int sellValue;
    public Rarity rarity; //rareity of the item, 4 common, 2 rare for now
    public int minAmount;
    public int maxAmount;
    [Range(0f, 1f)]
    public float dropChance; //randomise chance of drop
}
