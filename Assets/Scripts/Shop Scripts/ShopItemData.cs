using UnityEngine;

public enum ItemEffect { None, Medkit, SpeedBoost, DiverSuit }

[CreateAssetMenu(fileName = "ShopItem", menuName = "Shop/Item")]
public class ShopItemData : ScriptableObject
{
    public string     itemName;
    public string     description;
    public Sprite     icon;
    public int        price;
    public ItemEffect effect;
    public float      effectValue;    // speed multiplier (SpeedBoost) — ignored for Medkit
    public float      effectDuration; // seconds (SpeedBoost) — ignored for Medkit
}
