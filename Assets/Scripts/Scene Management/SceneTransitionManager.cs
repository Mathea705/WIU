using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject mainMenuPanel;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

     
            creditsPanel.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OpenMainMenu()
    {
         mainMenuPanel.SetActive(true);
         creditsPanel.SetActive(false);
    }

    public void OpenCredits()
    {
       
            creditsPanel.SetActive(true);
            mainMenuPanel.SetActive(false);
    }

    public void CloseCredits()
    {
    
            creditsPanel.SetActive(false);
            // mainMenuPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
