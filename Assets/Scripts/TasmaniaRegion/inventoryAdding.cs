using UnityEngine;
[System.Serializable]
public class InventoryAdding
{
    public int quantity;
    public LootingItem lootedItem;

    //add to inventory from chest
    public  InventoryAdding(LootingItem lootedItem, int quantity) //constructor
    {
        this.lootedItem = lootedItem;
        this.quantity = quantity;
    }
}
