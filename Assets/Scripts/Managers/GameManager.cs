using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public static event System.Action<GameObject> OnPlayerRegistered;

    [Header("Settings")]
    [SerializeField] private GameObject playerRoot;
    [SerializeField] private string worldScene;
    [SerializeField] private string sceneToLoad;

    [Header("Prefabs")]
    [SerializeField] private Sticky stickyMagnetProjectile;

    private string loadedAreaSceneName;
    private SceneReferences main;
    private SceneReferences current;

    private void Start()
    {
        PoolManager.Instance.InitializePool(stickyMagnetProjectile, 20);
    }

    public void GoToArea(string newSceneName)
    {
        main.SpawnPoint.position = playerRoot.transform.position;
        main.SpawnPoint.rotation = playerRoot.transform.rotation;

        sceneToLoad = newSceneName;
        loadedAreaSceneName = newSceneName;

        main.SetActiveGameObjects(false);

        CustomSceneManager.OnLoadedScene += CustomSceneManager_OnLoadedScene;
        CustomSceneManager.Instance.ChangeSceneTo(newSceneName);
    }

    private void CustomSceneManager_OnLoadedScene()
    {
        CustomSceneManager.OnLoadedScene -= CustomSceneManager_OnLoadedScene;
        SetAsActiveScene(sceneToLoad);
    }

    public void GoToMain()
    {
        main.SetActiveGameObjects(true);

        TeleportPlayerTo(main.SpawnPoint);

        Scene world = SceneManager.GetSceneByName(worldScene);
        SceneManager.SetActiveScene(world);

        if (!string.IsNullOrEmpty(loadedAreaSceneName))
        {
            SceneManager.UnloadSceneAsync(loadedAreaSceneName);
            loadedAreaSceneName = null;
        }
    }

    public void MovePlayer(SceneReferences sceneReferences)
    {
        if (main == null)
        {
            main = sceneReferences;
            current = sceneReferences;
        }
        else
        {
            current = sceneReferences;
            TeleportPlayerTo(current.SpawnPoint);
        }
    }

    private void TeleportPlayerTo(Transform target)
    {
        Rigidbody rb = playerRoot.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.position = target.position;
            rb.rotation = target.rotation;
        }
        else
        {
            playerRoot.transform.SetPositionAndRotation(target.position, target.rotation);
        }
    }

    private void SetAsActiveScene(string sceneName)
    {
        Scene newScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(newScene);
    }

    public void RegisterPlayer(GameObject player)
    {
        playerRoot = player;
        OnPlayerRegistered?.Invoke(player);
    }
}
