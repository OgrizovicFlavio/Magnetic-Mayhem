using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    private Vector3 previousPosition = Vector3.zero;

    [SerializeField] private GameObject playerRoot;
    [SerializeField] private string worldScene;
    [SerializeField] private string sceneToLoad;

    [Header("Prefabs")]
    [SerializeField] private Sticky stickyMagnetProjectile;

    private SceneReferences main;
    private SceneReferences current;
    private Vector3 returnPosition;
    private Quaternion returnRotation;

    private void Start()
    {
        if (playerRoot == null)
            playerRoot = GameObject.FindWithTag("Player");

        PoolManager.Instance.InitializePool(stickyMagnetProjectile, 20);
    }

    protected override void OnAwaken()
    {
        SceneReferences.OnLoadedScene += SceneReferences_OnLoadedScene;
    }

    protected override void OnDestroyed()
    {
        SceneReferences.OnLoadedScene -= SceneReferences_OnLoadedScene;
    }

    private void SceneReferences_OnLoadedScene(SceneReferences obj)
    {
        if (main == null)
            main = obj;
        else
        {
            current = obj;
            playerRoot.transform.position = current.returnPoint.position;
            playerRoot.transform.rotation = current.returnPoint.rotation;
        }
    }

    public void LoadScene(string newSceneName, Transform returnPoint)
    {
        returnPosition = returnPoint.position;
        returnRotation = returnPoint.rotation;

        main.returnPoint.position = playerRoot.transform.position;
        main.returnPoint.rotation = playerRoot.transform.rotation;

        sceneToLoad = newSceneName;

        main.SetActiveGameObjects(false);

        CustomSceneManager.OnLoadedScene += CustomSceneManager_OnLoadedScene;
        CustomSceneManager.Instance.ChangeSceneTo(newSceneName);
    }

    private void CustomSceneManager_OnLoadedScene()
    {
        CustomSceneManager.OnLoadedScene -= CustomSceneManager_OnLoadedScene;

        SetAsActiveScene(sceneToLoad);
    }

    public void UnloadScene()
    {
        SceneReferences.OnLoadedScene -= SceneReferences_OnLoadedScene;

        main.SetActiveGameObjects(true);

        playerRoot.transform.position = returnPosition;
        playerRoot.transform.rotation = returnRotation;

        Scene world = SceneManager.GetSceneByName(worldScene);
        SceneManager.SetActiveScene(world);

        SceneManager.UnloadSceneAsync(sceneToLoad);
    }

    private void SetAsActiveScene(string sceneName)
    {
        Scene newScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(newScene);
    }

    public Transform GetPlayerRoot()
    {
        return playerRoot != null ? playerRoot.transform : null;
    }
}
