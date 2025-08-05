using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string sceneToLoad = "World";

    [Header("Menus")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject controlsMenu;

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        if (playButton != null)
            playButton.onClick.AddListener(PlayGame);

        if (controlsButton != null)
            controlsButton.onClick.AddListener(ShowControls);

        if (backButton != null)
            backButton.onClick.AddListener(BackToMain);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    private void OnDisable()
    {
        if (playButton != null)
            playButton.onClick.RemoveAllListeners();

        if (controlsButton != null)
            controlsButton.onClick.RemoveAllListeners();

        if (backButton != null)
            backButton.onClick.RemoveAllListeners();

        if (quitButton != null)
            quitButton.onClick.RemoveAllListeners();
    }

    public void PlayGame()
    {
        Time.timeScale = 1;

        if (mainMenu != null)
            mainMenu.SetActive(false);
        if (controlsMenu != null)
            controlsMenu.SetActive(false);

        CustomSceneManager.Instance.ChangeSceneTo(sceneToLoad, false);
    }

    public void ShowControls()
    {
        if (mainMenu != null) mainMenu.SetActive(false);
        if (controlsMenu != null) controlsMenu.SetActive(true);
    }

    public void BackToMain()
    {
        if (mainMenu != null) mainMenu.SetActive(true);
        if (controlsMenu != null) controlsMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
