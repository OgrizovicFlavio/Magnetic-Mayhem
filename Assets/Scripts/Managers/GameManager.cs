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

    private void Start()
    {
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

            playerRoot.transform.position = current.previousState.position;
            playerRoot.transform.rotation = current.previousState.rotation;
        }
    }

    public void LoadScene(string newSceneName)
    {
        main.previousState.position = playerRoot.transform.position;
        main.previousState.rotation = playerRoot.transform.rotation;

        sceneToLoad = newSceneName;

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
        playerRoot.transform.position = main.previousState.position;
        playerRoot.transform.rotation = main.previousState.rotation;

        SetAsActiveScene(sceneToLoad);
        main.SetActiveGameObjects(true);

        Scene gameplay = SceneManager.GetSceneByName(worldScene);
        SceneManager.SetActiveScene(gameplay);
        SceneManager.UnloadSceneAsync(sceneToLoad);
    }

    private void SetAsActiveScene (string sceneName)
    {
        Scene newScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(newScene);

        main.SetActiveGameObjects(false);
    }

    public Transform GetPlayerRoot()
    {
        return playerRoot != null ? playerRoot.transform : null;
    }
}
