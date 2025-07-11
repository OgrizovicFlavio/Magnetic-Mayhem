using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomSceneManager : MonoBehaviourSingleton<CustomSceneManager>
{
    public static event Action OnLoadedScene;

    [Header("Settings")]
    [SerializeField] private Image image;
    [SerializeField] private GameObject background;
    [SerializeField] private float maxTime = 5f;

    private IEnumerator loadingScene;

    protected override void OnAwaken()
    {
        if (background != null)
            background.SetActive(false);

        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    public void ChangeSceneTo(string sceneName)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        if (loadingScene != null)
        {
            StopCoroutine(loadingScene);
            loadingScene = null;
        }

        if (background != null)
            background.SetActive(true);

        image.fillAmount = 0f;

        loadingScene = LoadingScene(sceneName);
        Time.timeScale = 0;
        StartCoroutine(loadingScene);
    }

    private IEnumerator LoadingScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        operation.allowSceneActivation = false;

        float onTime = 0f;
        float percentage = 0.9f;

        while (onTime < maxTime * percentage)
        {
            onTime += Time.unscaledDeltaTime;
            image.fillAmount = onTime / maxTime;
            yield return null;
        }

        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        while (onTime < maxTime)
        {
            onTime += Time.unscaledDeltaTime * 10f;
            image.fillAmount = onTime / maxTime;
            yield return null;
        }

        image.fillAmount = 1f;

        gameObject.SetActive(false);
        Time.timeScale = 1;

        operation.allowSceneActivation = true;

        yield return null;

        image.fillAmount = 0f;

        if (background != null)
            background.SetActive(false);

        OnLoadedScene?.Invoke();
    }
}

