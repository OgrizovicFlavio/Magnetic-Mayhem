using UnityEngine;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button winMenuButton;
    [SerializeField] private Button winCreditsButton;
    [SerializeField] private Button loseMenuButton;
    [SerializeField] private Button loseCreditsButton;
    [SerializeField] private Button closeCreditsButton;

    [Header("Panels")]
    [SerializeField] private GameObject creditsPanel;

    private GameObject lastPanelShown;

    private void OnEnable()
    {
        winMenuButton?.onClick.AddListener(GoToMainMenu);
        winCreditsButton?.onClick.AddListener(ShowCredits);

        loseMenuButton?.onClick.AddListener(GoToMainMenu);
        loseCreditsButton?.onClick.AddListener(ShowCredits);

        closeCreditsButton?.onClick.AddListener(CloseCredits);
    }

    private void OnDisable()
    {
        winMenuButton?.onClick.RemoveListener(GoToMainMenu);
        winCreditsButton?.onClick.RemoveListener(ShowCredits);

        loseMenuButton?.onClick.RemoveListener(GoToMainMenu);
        loseCreditsButton?.onClick.RemoveListener(ShowCredits);

        closeCreditsButton?.onClick.RemoveListener(CloseCredits);
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        CustomSceneManager.Instance.ChangeSceneTo("Main Menu");
    }

    private void ShowCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(true);

        if (winMenuButton.transform.parent.gameObject.activeSelf)
        {
            lastPanelShown = winMenuButton.transform.parent.gameObject;
            lastPanelShown.SetActive(false);
        }
        else if (loseMenuButton.transform.parent.gameObject.activeSelf)
        {
            lastPanelShown = loseMenuButton.transform.parent.gameObject;
            lastPanelShown.SetActive(false);
        }
    }

    private void CloseCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        if (lastPanelShown != null)
        {
            lastPanelShown.SetActive(true);
            lastPanelShown = null;
        }
    }
}
