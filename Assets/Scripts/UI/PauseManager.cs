using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviourSingleton<PauseManager>
{
    public static event Action<bool> OnPause;

    [Header("UI")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject crosshair;

    private bool isPaused = false;

    protected override void OnAwaken()
    {
        Time.timeScale = 1;
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (crosshair != null) crosshair.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();

            OnPause?.Invoke(isPaused);
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;

        if (pauseMenu != null) pauseMenu.SetActive(true);
        if (crosshair != null) crosshair.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;

        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (crosshair != null) crosshair.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        CustomSceneManager.Instance.ChangeSceneTo("Main Menu");
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
