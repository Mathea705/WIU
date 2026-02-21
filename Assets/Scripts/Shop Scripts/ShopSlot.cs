using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlot : MonoBehaviour
{
    [SerializeField] private Image  icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button   buyButton;

    [SerializeField] private TMP_Text buyText;

    public static readonly List<GameObject> PendingGuns = new List<GameObject>();
    public static readonly List<GameObject> SoldGuns    = new List<GameObject>();
    public static bool GunOwned;

    private AurumManager     _aurum;
    private ShopItemData     _data;
    private GameObject       _gunObject;
    private HealthSystem     _health;
    private PlayerController _player;
    private StaminaSystem    _stamina;
    private bool             _purchased;


    public void Setup(ShopItemData data, AurumManager aurum, GameObject gunObject = null, bool alreadyOwned = false,
                      HealthSystem health = null, PlayerController player = null, StaminaSystem stamina = null)
    {
        _data      = data;
        _aurum     = aurum;
        _gunObject = gunObject;
        _health    = health;
        _player    = player;
        _stamina   = stamina;

        icon.sprite    = data.icon;
        nameText.text  = data.itemName;
        descText.text  = data.description;
        priceText.text = data.price.ToString();

        buyButton.onClick.AddListener(TryBuy);

        if (alreadyOwned)
        {
            _purchased             = true;
            buyButton.interactable = false;
            buyText.text           = "BOUGHT";
        }
    }

    private void TryBuy()
    {
        if (_purchased) return;
        if (_gunObject != null && GunOwned) return;
        if (!_aurum.DeductAurum(_data.price)) return;

        // Consumables: apply effect and leave button active
        switch (_data.effect)
        {
            case ItemEffect.Medkit:
                if (_health != null) _health.HealFull();
                return;
            case ItemEffect.SpeedBoost:
                if (_player != null) _player.ApplySpeedBoost(_data.effectValue, _data.effectDuration);
                return;
            case ItemEffect.DiverSuit:
                if (_stamina != null) _stamina.ApplyDiverBoost();
                _purchased             = true;
                buyButton.interactable = false;
                buyText.text           = "BOUGHT";
                return;
        }

        // Gun purchase — lock the slot
        _purchased             = true;
        buyButton.interactable = false;
        buyText.text           = "BOUGHT";

        if (_gunObject != null)
        {
            GunOwned = true;
            PendingGuns.Add(_gunObject);
        }
    }

    public void SetupSell(ShopItemData data, AurumManager aurum, GameObject gun)
    {
        _data      = data;
        _aurum     = aurum;
        _gunObject = gun;

        if (data.icon != null) icon.sprite = data.icon;
        nameText.text  = data.itemName;
        descText.text  = data.description;
        priceText.text = data.price.ToString();
        buyText.text   = "SELL";

        buyButton.onClick.AddListener(TrySell);
    }

    private void TrySell()
    {
        if (_purchased) return;

        _purchased             = true;
        buyButton.interactable = false;
        buyText.text           = "SOLD";

        _aurum.AddAurum(_data.price);
        if (_gunObject != null) SoldGuns.Add(_gunObject);
        GunOwned = false;
    }
}
