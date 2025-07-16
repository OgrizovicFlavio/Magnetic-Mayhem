using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "World";
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject controlsMenu;

    public void PlayGame()
    {
        Time.timeScale = 1;
        CustomSceneManager.Instance.ChangeSceneTo(sceneToLoad);
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
