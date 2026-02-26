using UnityEngine;

public class SceneMusicPlayer : MonoBehaviour
{
    [SerializeField] private string musicName;

    private void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic(musicName);
    }

    private void OnDestroy()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopMusic();
    }
}
