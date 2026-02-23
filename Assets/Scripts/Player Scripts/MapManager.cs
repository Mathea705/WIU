using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject mapCanvas;

    [SerializeField] private GameObject[] mapPanels;

    private bool isActive;
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.M) && isActive)
        {
            DeactivateMap();
        }
        else if (Input.GetKeyDown(KeyCode.M) && !isActive) {
            ActivateMap();
        }

    }

    void CheckActive()
    {
        if (isActive)
        {
             Cursor.lockState = CursorLockMode.None;
             Cursor.visible   = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }
    
    }

    void ActivateMap()
    {
        isActive = true;
       mapCanvas.SetActive(true);
       CheckActive();
    }

    void DeactivateMap()
    {
        isActive = false;
         mapCanvas.SetActive(false);
          CheckActive();
    }

    public void DisplayDocks()
    {
        mapPanels[0].SetActive(true);
        for (int i = 1; i < mapPanels.Length; i++) {
            mapPanels[i].SetActive(false);
        }
    }

      public void DisplayBali()
    {

        for (int i = 0; i < mapPanels.Length; i++) {
            mapPanels[i].SetActive(false);
        }

        mapPanels[1].SetActive(true);
    }

       public void DisplayTasmania()
    {

        for (int i = 0; i < mapPanels.Length; i++) {
            mapPanels[i].SetActive(false);
        }

        mapPanels[2].SetActive(true);
    }

       public void DisplayBermuda()
    {

        for (int i = 0; i < mapPanels.Length; i++) {
            mapPanels[i].SetActive(false);
        }

        mapPanels[3].SetActive(true);
    }

       public void DisplaySunkenCity()
    {

        for (int i = 0; i < mapPanels.Length; i++) {
            mapPanels[i].SetActive(false);
        }

        mapPanels[4].SetActive(true);
    }
}
