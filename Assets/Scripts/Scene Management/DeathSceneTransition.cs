using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathSceneTransition : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void GoToHub()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
