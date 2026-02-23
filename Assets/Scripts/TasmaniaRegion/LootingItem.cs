using UnityEngine;
public enum Rarity
{
    Common,
    Rare
}
[CreateAssetMenu(fileName = "LootingItem", menuName = "Scriptable Objects/LootingItem1")]
public class LootingItem : ScriptableObject
{
    public string itemName;
    public int sellValue;
    public Rarity rarity; //rareity of the item, 4 common, 2 rare for now
    public int minAmount;
    public int maxAmount;
    //[Range(0f, 1f)]

    //public float dropChance; //randomise chance of drop
}
