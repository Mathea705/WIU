using UnityEngine;
using UnityEngine.SceneManagement;


public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    [SerializeField] private GameObject[] persistentObjects;

    [SerializeField] private GameObject ship;
   
    [SerializeField] private Vector3 playerOnShipOffset = new Vector3(0f, 2f, 0f);

    [SerializeField] private string mainSceneName   = "MainScene";
    [SerializeField] private string startRegionName = "";

    public string PendingRegionName { get; set; }

    public Vector3  ReturnPosition { get; set; }
    public Quaternion ReturnRotation { get; set; }
    public bool HasReturnPoint { get; private set; }


    public Vector3  ArrivalPosition { get; set; }
    public Quaternion ArrivalRotation { get; set; }
    public bool HasArrivalPoint { get; private set; }


    void Awake()
    {
  
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (GameObject obj in persistentObjects)
        {
            if (obj != null)
                DontDestroyOnLoad(obj);
        }
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(startRegionName))
            if (RegionBannerUI.Instance != null) RegionBannerUI.Instance.Show(startRegionName);
    }

    void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;



    public void LoadScene(string sceneName)
    {
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene(sceneName);
        else
            SceneManager.LoadScene(sceneName);
    }

    public void SetReturnPoint(Vector3 returnPos, Quaternion returnRot)
    {
        ReturnPosition  = returnPos;
        ReturnRotation  = returnRot;
        HasReturnPoint  = true;
    }

    public void SetArrivalPoint(Vector3 arrivalPos, Quaternion arrivalRot)
    {
        ArrivalPosition  = arrivalPos;
        ArrivalRotation  = arrivalRot;
        HasArrivalPoint  = true;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = FindPlayer();
        if (player == null) return;

        // Destroy any scene-native players — the persistent one is in DontDestroyOnLoad.
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
            if (p.scene.name != "DontDestroyOnLoad")
                Destroy(p);

        // Show region banner for every scene load
        if (!string.IsNullOrEmpty(PendingRegionName))
        {
            if (RegionBannerUI.Instance != null) RegionBannerUI.Instance.Show(PendingRegionName);
            PendingRegionName = null;
        }
        else if (scene.name == mainSceneName && !string.IsNullOrEmpty(startRegionName))
        {
            if (RegionBannerUI.Instance != null) RegionBannerUI.Instance.Show(startRegionName);
        }

        if (scene.name == mainSceneName)
        {
            // Destroy any scene-native ships that got recreated — the persistent one is already here.
            foreach (GameObject sceneShip in GameObject.FindGameObjectsWithTag("Ship"))
                if (sceneShip != ship) Destroy(sceneShip);

            // Destroy any scene-native screen cameras — persistent one lives in "DontDestroyOnLoad".
            // Skip cameras that render to a RenderTexture (ripple cam, reflections, etc.).
            foreach (Camera cam in FindObjectsByType<Camera>(FindObjectsSortMode.None))
                if (cam.gameObject.scene.name != "DontDestroyOnLoad" && cam.targetTexture == null)
                    Destroy(cam.gameObject);

            // Destroy scene-native duplicates of every persistent object (canvases, etc.).
            // Only check root GameObjects of the loaded scene — avoids false child matches.
            foreach (GameObject root in scene.GetRootGameObjects())
                foreach (GameObject persistent in persistentObjects)
                    if (persistent != null && root.name == persistent.name)
                        Destroy(root);

            if (HasReturnPoint)
            {
                MoveToSpawn(player, ReturnPosition, ReturnRotation);
                HasReturnPoint = false;
            }
        }
        else
        {
            if (HasArrivalPoint)
            {
                MoveToSpawn(player, ArrivalPosition, ArrivalRotation);
                HasArrivalPoint = false;
            }
            else
            {
                GameObject spawnObj = GameObject.FindWithTag("PlayerSpawn");
                if (spawnObj != null)
                    MoveToSpawn(player, spawnObj.transform.position, spawnObj.transform.rotation);
            }
        }
    }


    private void MoveToSpawn(GameObject player, Vector3 spawnPos, Quaternion spawnRot)
    {
        // Zero velocity first so carried momentum doesn't throw the player off the ship.
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }

        if (ship != null)
        {
            ship.transform.position = spawnPos;
            ship.transform.rotation = spawnRot;
            player.transform.position = spawnPos + playerOnShipOffset;
        }
        else
        {
            player.transform.position = spawnPos;
        }
        player.transform.rotation = spawnRot;
    }



    private GameObject FindPlayer()
    {
        // Check the persistent array first so we never accidentally return a scene-native duplicate.
        foreach (GameObject obj in persistentObjects)
            if (obj != null && obj.CompareTag("Player"))
                return obj;

        return GameObject.FindWithTag("Player");
    }

    
}
