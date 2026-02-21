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

    private AurumManager _aurum;
    private ShopItemData _data;
    private bool         _purchased;

    public void Setup(ShopItemData data, AurumManager aurum)
    {
        _data  = data;
        _aurum = aurum;

        if (data.icon != null) icon.sprite = data.icon;
        nameText.text  = data.itemName;
        descText.text  = data.description;
        priceText.text = data.price.ToString();

        buyButton.onClick.AddListener(TryBuy);
    }

    private void TryBuy()
    {
        if (_purchased) return;
        if (!_aurum.DeductAurum(_data.price)) return;

        _purchased             = true;
        buyButton.interactable = false;
        // priceText.text         = "Purchased";
        buyText.text = "BOUGHT";

    }
}
