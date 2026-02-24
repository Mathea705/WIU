using UnityEngine;

public class sellbutton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSellButtonPressed() //issue with selling looted stuff from other scenes
    {
        InventoryManager.instance.SellAll();
    }
}
