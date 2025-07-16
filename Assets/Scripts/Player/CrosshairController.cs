using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image crosshairImage;

    [Header("Sprites")]
    [SerializeField] private Sprite canShootSprite;
    [SerializeField] private Sprite cannotShootSprite;

    private MagneticChargeType currentCharge = MagneticChargeType.Positive;
    private bool canShoot = true;

    public void SetCharge(MagneticChargeType charge)
    {
        currentCharge = charge;
        UpdateVisuals();
    }

    public void SetCanShoot(bool value)
    {
        canShoot = value;
        UpdateVisuals();
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    private void UpdateVisuals()
    {
        if (canShoot)
            crosshairImage.sprite = canShootSprite;
        else
            crosshairImage.sprite = cannotShootSprite;

        if (currentCharge == MagneticChargeType.Positive)
            crosshairImage.color = Color.red;
        else
            crosshairImage.color = Color.blue;
    }
}
