using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviourSingleton<UIManager>
{
    [Header("Health")]
    [SerializeField] private Image healthFill;

    [Header("Interaction Hint")]
    [SerializeField] private GameObject interactionHintRoot;
    [SerializeField] private TMP_Text interactionHintText;

    protected override void OnAwaken()
    {
        if (interactionHintRoot != null)
            interactionHintRoot.SetActive(false);
    }

    public void SetHealth(float current, float max)
    {
        if (healthFill != null)
            healthFill.fillAmount = current / max;
    }

    public void ShowInteractionHint(string message)
    {
        if (interactionHintRoot != null)
            interactionHintRoot.SetActive(true);

        if (interactionHintText != null)
            interactionHintText.text = message;
    }

    public void HideInteractionHint()
    {
        if (interactionHintRoot != null)
            interactionHintRoot.SetActive(false);
    }
}
