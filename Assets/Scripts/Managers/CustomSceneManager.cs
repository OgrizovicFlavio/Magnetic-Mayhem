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

        if (image != null)
        {
            image.fillAmount = 0f;
            image.enabled = false;
        }
    }

    public void ChangeSceneTo(string sceneName)
    {
        if (loadingScene != null)
        {
            StopCoroutine(loadingScene);
            loadingScene = null;
        }

        if (background != null)
            background.SetActive(true);

        if (image != null)
        {
            image.fillAmount = 0f;
            image.enabled = true;
        }

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
            if (image != null)
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
            if (image != null)
                image.fillAmount = onTime / maxTime;

            yield return null;
        }

        if (image != null)
        {
            image.fillAmount = 1f;
            image.enabled = false;
        }

        if (background != null)
            background.SetActive(false);

        Time.timeScale = 1;
        operation.allowSceneActivation = true;

        yield return null;

        OnLoadedScene?.Invoke();
    }
}

