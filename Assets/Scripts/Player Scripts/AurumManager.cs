using UnityEngine;
using TMPro;

public class AurumManager : MonoBehaviour
{
    [SerializeField] private int startingAurum = 0;
    [SerializeField] private TMP_Text aurumTextHUD;
    [SerializeField] private TMP_Text aurumTextShop;

    public int Aurum { get; private set; }

    private void Start()
    {
        Aurum = startingAurum;
        UpdateDisplay();
    }

    public void AddAurum(int amount)
    {
        Aurum += amount;
        UpdateDisplay();
    }

    public bool DeductAurum(int amount)
    {
        if (Aurum < amount) return false;

        Aurum -= amount;
        UpdateDisplay();
        return true;
    }

    public bool CanAfford(int amount) => Aurum >= amount;

    private void UpdateDisplay()
    {
        aurumTextHUD.text  = Aurum.ToString();
        if (aurumTextShop != null) aurumTextShop.text = Aurum.ToString();
        
    }
}
