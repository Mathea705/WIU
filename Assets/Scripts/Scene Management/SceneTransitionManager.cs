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
            AudioManager.Instance.PlayMusic("MainMenuMusic");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
        AudioManager.Instance.Play("ButtonClickSFX");
          AudioManager.Instance.StopMusic();
    }

    public void OpenMainMenu()
    {
         mainMenuPanel.SetActive(true);
         creditsPanel.SetActive(false);
          AudioManager.Instance.Play("ButtonClickSFX");
    }

    public void OpenCredits()
    {
       
            creditsPanel.SetActive(true);
            mainMenuPanel.SetActive(false);
              AudioManager.Instance.Play("ButtonClickSFX");
    }

    public void CloseCredits()
    {
    
            creditsPanel.SetActive(false);
              AudioManager.Instance.Play("ButtonClickSFX");
            // mainMenuPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
          AudioManager.Instance.Play("ButtonClickSFX");
    }
}
