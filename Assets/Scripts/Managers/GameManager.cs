using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public static event System.Action<GameObject> OnPlayerRegistered;

    [Header("Settings")]
    [SerializeField] private GameObject playerRoot;
    [SerializeField] private string worldScene;
    [SerializeField] private string sceneToLoad;

    [Header("Game States")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Prefabs")]
    [SerializeField] private Sticky stickyMagnetProjectile;

    private string loadedAreaSceneName;
    private SceneReferences main;
    private SceneReferences current;
    private bool gameEnded = false;
    private bool completedArea1 = false;
    private bool completedArea2 = false;

    public bool IsTransitioning { get; set; }

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
        CustomSceneManager.Instance.ChangeSceneTo(newSceneName, true);
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

    public void RegisterLevelCompleted(string areaName)
    {
        if (areaName == "Area 1") completedArea1 = true;
        else if (areaName == "Area 2") completedArea2 = true;

        if (completedArea1 && completedArea2)
            WinGame();
    }

    public void WinGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        Time.timeScale = 0f;

        if (winPanel != null)
            winPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoseGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        Time.timeScale = 0f;

        if (losePanel != null)
            losePanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }

    public void RegisterPlayer(GameObject player)
    {
        playerRoot = player;
        OnPlayerRegistered?.Invoke(player);
    }

    public GameObject GetPlayer()
    {
        return playerRoot;
    }

    public bool IsGameEnded()
    {
        return gameEnded;
    }
}

