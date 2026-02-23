using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{

    [SerializeField] private string targetScene;

 
    [SerializeField] private Transform returnSpawnPoint;


    [SerializeField] private Transform arrivalSpawnPoint;




    private bool _triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;
        if (!other.CompareTag("Player")) return;
        _triggered = true;
        ExecuteLoad();
    }


    private void ExecuteLoad()
    {
        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogWarning($"[SceneTrigger] '{gameObject.name}' has no Target Scene set.", this);
            return;
        }


        if (GameSceneManager.Instance != null)
        {
          
            if (returnSpawnPoint != null)
                GameSceneManager.Instance.SetReturnPoint(returnSpawnPoint.position, returnSpawnPoint.rotation);

     
            if (arrivalSpawnPoint != null)
                GameSceneManager.Instance.SetArrivalPoint(arrivalSpawnPoint.position, arrivalSpawnPoint.rotation);

            GameSceneManager.Instance.LoadScene(targetScene);
        }
        else
        {
           
            SceneManager.LoadScene(targetScene);
        }
    }
}
